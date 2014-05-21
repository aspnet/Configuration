// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.Framework.OptionsModel
{
    public class OptionsServices
    {
        public static IEnumerable<IServiceDescriptor> GetDefaultServices()
        {
            return GetDefaultServices(new Configuration());
        }

        public static IEnumerable<IServiceDescriptor> GetDefaultServices(IConfiguration configuration)
        {
            var describe = new ServiceDescriber(configuration);

            yield return describe.Describe(
                typeof(IOptionsAccessor<>),
                typeof(OptionsAccessor<>),
                null,
                LifecycleKind.Singleton);
        }

// No convert on portable
#if NET45 || K10
        public static void ReadProperties(object obj, IConfiguration config)
        {
            if (obj == null || config == null)
            {
                return;
            }
            var props = obj.GetType().GetTypeInfo().DeclaredProperties;
            foreach (var prop in props)
            {
                if (!prop.CanWrite)
                {
                    continue;
                }
                var configValue = config.Get(prop.Name);
                if (configValue == null)
                {
                    continue;
                }

                // TODO: what do we do about errors?
                prop.SetValue(obj, Convert.ChangeType(configValue, prop.PropertyType));
            }
        }
#endif
    }
}