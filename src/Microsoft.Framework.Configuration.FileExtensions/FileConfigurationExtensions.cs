// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Microsoft.Framework.Configuration
{
    public static class FileConfigurationExtensions
    {
        /// <summary>
        /// Sets the base path in <see cref="IConfigurationBuilder"/> for file based providers.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="basePath">Absolute path of file based providers.
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder SetBasePath(this IConfigurationBuilder builder, string basePath)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            builder.Properties["BasePath"] = basePath;
            
            return builder;
        }

        /// <summary>
        /// Gets the base path in <see cref="IConfigurationBuilder"/> for file based providers.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static string GetBasePath(this IConfigurationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            object basePath;
            if (builder.Properties.TryGetValue("BasePath", out basePath))
            {
                return (string)basePath;
            }

            return null;
        }

        public static string GetConfigurationFilePath(this IConfigurationBuilder configuration, string path)
        {
            object value;
            var basePath = string.Empty;

            if (configuration.Properties.TryGetValue("BasePath", out value))
            {
                basePath = (string)value;
            }

            path = Path.Combine(basePath, path);

            return path;
        }
    }
}
