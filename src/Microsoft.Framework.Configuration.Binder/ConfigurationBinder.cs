// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Framework.Configuration.Binder;

namespace Microsoft.Framework.Configuration
{
    public static class ConfigurationBinder
    {
        public static void Bind(this IConfiguration configuration, object model)
        {
            if (model != null)
            {
                foreach (var property in GetAllProperties(model.GetType().GetTypeInfo()))
                {
                    BindProperty(property, model, configuration);
                }
            }
        }

        private static void BindProperty(PropertyInfo property, object instance, IConfiguration config)
        {
            // We don't support set only, non public, or indexer properties
            if (property.GetMethod == null ||
                !property.GetMethod.IsPublic ||
                property.GetMethod.GetParameters().Length > 0)
            {
                return;
            }

            var propertyValue = property.GetValue(instance);
            var hasPublicSetter = property.SetMethod != null && property.SetMethod.IsPublic;

            if (propertyValue == null && !hasPublicSetter)
            {
                // Property doesn't have a value and we cannot set it so there is no
                // point in going further down the graph
                return;
            }

            propertyValue = BindInstance(property.PropertyType, propertyValue, config.GetSection(property.Name));
            if (propertyValue != null && hasPublicSetter)
            {
                property.SetValue(instance, propertyValue);
            }
        }

        private static object BindInstance(Type type, object instance, IConfiguration config)
        {
            var section = config as IConfigurationSection;
            var configValue = section?.Value;
            if (configValue != null)
            {
                // Leaf nodes are always reinitialized
                return ReadValue(type, configValue, section);
            }

            if (config.GetChildren().Any())
            {
                if (instance == null)
                {
                    instance = CreateInstance(type);
                }

                // See if its a Dictionary
                var collectionInterface = FindOpenGenericInterface(typeof(IDictionary<,>), type);
                if (collectionInterface != null)
                {
                    BindDictionary(instance, collectionInterface, config);
                }
                else if (type.IsArray)
                {
                    instance = BindArray((Array)instance, config);
                }
                else
                {
                    // See if its an ICollection
                    collectionInterface = FindOpenGenericInterface(typeof(ICollection<>), type);
                    if (collectionInterface != null)
                    {
                        BindCollection(instance, collectionInterface, config);
                    }
                    // Something else
                    else
                    {
                        Bind(config, instance);
                    }
                }
            }

            return instance;
        }

        private static object CreateInstance(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            
            if (typeInfo.IsInterface || typeInfo.IsAbstract)
            {
                throw new InvalidOperationException(Resources.FormatError_CannotActivateAbstractOrInterface(type));
            }

            if (typeInfo.IsArray)
            {
                if (typeInfo.GetArrayRank() > 1)
                {
                    throw new InvalidOperationException(Resources.FormatError_UnsupportedMultidimensionalArray(type));
                }

                return Array.CreateInstance(typeInfo.GetElementType(), 0);
            }
            
            var hasDefaultConstructor = typeInfo.DeclaredConstructors.Any(ctor => ctor.IsPublic && ctor.GetParameters().Length == 0);
            if (!hasDefaultConstructor)
            {
                throw new InvalidOperationException(Resources.FormatError_MissingParameterlessConstructor(type));
            }

            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(Resources.FormatError_FailedToActivate(type), ex);
            }
        }

        private static void BindDictionary(object dictionary, Type dictionaryType, IConfiguration config)
        {
            var typeInfo = dictionaryType.GetTypeInfo();

            // IDictionary<K,V> is guaranteed to have exactly two parameters
            var keyType = typeInfo.GenericTypeArguments[0];
            var valueType = typeInfo.GenericTypeArguments[1];

            if (keyType != typeof(string))
            {
                // We only support string keys
                return;
            }

            var addMethod = typeInfo.GetDeclaredMethod("Add");
            foreach (var child in config.GetChildren())
            {
                var item = BindInstance(
                    type: valueType,
                    instance: null,
                    config: child);
                if (item != null)
                {
                    var key = child.Key;
                    var section = config as IConfigurationSection;
                    if (section != null)
                    {
                        // Remove the parent key and : delimiter to get the configurationSection's key
                        key = key.Substring(section.Key.Length + 1);
                    }

                    addMethod.Invoke(dictionary, new[] { key, item });
                }
            }
        }

        private static void BindCollection(object collection, Type collectionType, IConfiguration config)
        {
            var typeInfo = collectionType.GetTypeInfo();

            // ICollection<T> is guaranteed to have exacly one parameter
            var itemType = typeInfo.GenericTypeArguments[0];
            var addMethod = typeInfo.GetDeclaredMethod("Add");

            foreach (var section in config.GetChildren())
            {
                try
                {
                    var item = BindInstance(
                        type: itemType,
                        instance: null,
                        config: section);
                    if (item != null)
                    {
                        addMethod.Invoke(collection, new[] { item });
                    }
                }
                catch
                {
                }
            }
        }

        private static Array BindArray(Array source, IConfiguration config)
        {
            var children = config.GetChildren().ToArray();
            var arrayLength = source.Length;
            var elementType = source.GetType().GetElementType();
            var newArray = Array.CreateInstance(elementType, arrayLength + children.Length);

            // binding to array has to preserve already initialized arrays with values
            if (arrayLength > 0)
            {
                Array.Copy(source, newArray, arrayLength);
            }

            for(int i = 0; i < children.Length; i++)
            {
                try
                {
                    var item = BindInstance(
                        type: elementType,
                        instance: null,
                        config: children[i]);
                    if (item != null)
                    {
                        newArray.SetValue(item, arrayLength + i);
                    }
                }
                catch
                {
                }
            }

            return newArray;
        }

        private static object ReadValue(Type type, string value, IConfigurationSection config)
        {
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return ReadValue(Nullable.GetUnderlyingType(type), value, config);
            }

            try
            {
                return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(config.Value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(Resources.FormatError_FailedBinding(config.Value, type), ex);
            }
        }

        private static Type FindOpenGenericInterface(Type expected, Type actual)
        {
            var interfaces = actual.GetTypeInfo().ImplementedInterfaces;
            foreach (var interfaceType in interfaces)
            {
                if (interfaceType.GetTypeInfo().IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == expected)
                {
                    return interfaceType;
                }
            }
            return null;
        }

        private static IEnumerable<PropertyInfo> GetAllProperties(TypeInfo type)
        {
            var allProperties = new List<PropertyInfo>();

            do
            {
                allProperties.AddRange(type.DeclaredProperties);
                type = type.BaseType.GetTypeInfo();
            }
            while (type != typeof(object).GetTypeInfo());

            return allProperties;
        }
    }
}
