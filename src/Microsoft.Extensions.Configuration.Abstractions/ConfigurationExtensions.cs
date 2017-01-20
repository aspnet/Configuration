// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for <see cref="IConfiguration" />.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Shorthand for GetSection("ConnectionStrings")[name].
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="name">The connection string key.</param>
        /// <returns></returns>
        public static string GetConnectionString(this IConfiguration configuration, string name)
        {
            return configuration?.GetSection("ConnectionStrings")?[name];
        }

        /// <summary>
        /// Get the enumeration of key value pairs within the <see cref="IConfiguration" />
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> to enumerate.</param>
        /// <returns>An enumeration of key value pairs.</returns>
        public static IEnumerable<KeyValuePair<string, string>> AsEnumerable(this IConfiguration configuration) => configuration.AsEnumerable(makePathsRelative: false);

        /// <summary>
        /// Get the enumeration of key value pairs within the <see cref="IConfiguration" />
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> to enumerate.</param>
        /// <param name="makePathsRelative">If true, the child keys returned will have the current configuration's Path trimmed from the front.</param>
        /// <returns>An enumeration of key value pairs.</returns>
        public static IEnumerable<KeyValuePair<string, string>> AsEnumerable(this IConfiguration configuration, bool makePathsRelative)
        {
            var stack = new Stack<IConfiguration>();
            stack.Push(configuration);
            var rootSection = configuration as IConfigurationSection;
            var prefixLength = (makePathsRelative && rootSection != null) ? rootSection.Path.Length + 1 : 0;
            while (stack.Count > 0)
            {
                var config = stack.Pop();
                var section = config as IConfigurationSection;
                // Don't include the sections value if we are removing paths, since it will be an empty key
                if (section != null && (!makePathsRelative || config != configuration))
                {
                    yield return new KeyValuePair<string, string>(section.Path.Substring(prefixLength), section.Value);
                }
                foreach (var child in config.GetChildren())
                {
                    stack.Push(child);
                }
            }
        }

        /// <summary>
        /// Determines whether the section has a <see cref="IConfigurationSection.Value"/> or has children 
        /// </summary>
        public static bool Exists(this IConfigurationSection section)
        {
            if (section == null)
            {
                return false;
            }
            return section.Value != null || section.GetChildren().Any();
        }

        /// <summary>
        /// Used to call action safely using OnLoadException to handle exceptions if specified.
        /// </summary>
        /// <param name="provider">The provider's OnLoadException to use.</param>
        /// <param name="action">The action to invoke.</param>
        public static void SafeInvoke(this IConfigurationProvider provider, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                bool ignoreException = false;
                if (provider.Source.OnLoadException != null)
                {
                    var exceptionContext = new ConfigurationLoadExceptionContext
                    {
                        Provider = provider,
                        Exception = e
                    };
                    provider.Source.OnLoadException.Invoke(exceptionContext);
                    ignoreException = exceptionContext.Ignore;
                }
                if (!ignoreException)
                {
                    throw e;
                }
            }
        }


        /// <summary>
        /// Used to call Load safely using OnLoadException to handle exceptions if specified.
        /// </summary>
        /// <param name="provider"></param>
        public static void SafeLoad(this IConfigurationProvider provider) => provider.SafeInvoke(provider.Load);

        private static string LoadExceptionHandlerKey = "LoadExceptionHandler";

        /// <summary>
        /// Sets a default action to be invoked when an error occurs during load.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="handler">The Action to be invoked on a load exception.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder SetLoadExceptionHandler(this IConfigurationBuilder builder, Action<ConfigurationLoadExceptionContext> handler)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Properties[LoadExceptionHandlerKey] = handler;
            return builder;
        }

        /// <summary>
        /// Gets the default action to be invoked when an error occurs during load.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>The Action to be invoked on a load exception.</returns>
        public static Action<ConfigurationLoadExceptionContext> GetLoadExceptionHandler(this IConfigurationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            object handler;
            if (builder.Properties.TryGetValue(LoadExceptionHandlerKey, out handler))
            {
                return builder.Properties[LoadExceptionHandlerKey] as Action<ConfigurationLoadExceptionContext>;
            }
            return null;
        }
    }
}