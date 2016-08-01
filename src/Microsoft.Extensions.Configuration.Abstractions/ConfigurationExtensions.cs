// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

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
        public static IEnumerable<KeyValuePair<string, string>> AsEnumerable(this IConfiguration configuration) => configuration.AsEnumerable(removePathFromChildKeys: false);

        /// <summary>
        /// Get the enumeration of key value pairs within the <see cref="IConfiguration" />
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> to enumerate.</param>
        /// <param name="removePathFromChildKeys">If true, removes the configuration path from the child keys.</param>
        /// <returns>An enumeration of key value pairs.</returns>
        public static IEnumerable<KeyValuePair<string, string>> AsEnumerable(this IConfiguration configuration, bool removePathFromChildKeys)
        {
            var stack = new Stack<IConfiguration>();
            stack.Push(configuration);
            var rootSection = configuration as IConfigurationSection;
            var prefixLength = (removePathFromChildKeys && rootSection != null) ? rootSection.Path.Length + 1 : 0;
            while (stack.Count > 0)
            {
                var config = stack.Pop();
                var section = config as IConfigurationSection;
                // Don't include the sections value if we are removing paths, since it will be an empty key
                if (section != null && (!removePathFromChildKeys || config != configuration))
                {
                    yield return new KeyValuePair<string, string>(section.Path.Substring(prefixLength), section.Value);
                }
                foreach (var child in config.GetChildren())
                {
                    stack.Push(child);
                }
            }
        }
    }
}