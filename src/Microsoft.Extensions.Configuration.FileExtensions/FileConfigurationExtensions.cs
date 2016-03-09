// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration
{
    public static class FileConfigurationExtensions
    {
        /// <summary>
        /// Sets the FileProvider for file-based providers to a PhysicalFileProvider with the base path.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="basePath">The absolute path of file-based providers.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder SetFileProvider(this IConfigurationBuilder configurationBuilder, string basePath)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            configurationBuilder.Properties["FileProvider"] = new PhysicalFileProvider(basePath);

            return configurationBuilder;
        }

        /// <summary>
        /// Gets the IFileProvider to use for file-based providers.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <returns>The file provider.</returns>
        public static IFileProvider GetFileProvider(this IConfigurationBuilder configurationBuilder)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            object fileProvider;
            if (configurationBuilder.Properties.TryGetValue("FileProvider", out fileProvider))
            {
                return (IFileProvider)fileProvider;
            }

#if NET451
            var stringBasePath = AppDomain.CurrentDomain.GetData("APP_CONTEXT_BASE_DIRECTORY") as string ??
                AppDomain.CurrentDomain.BaseDirectory ?? 
                string.Empty;

            return new PhysicalFileProvider(stringBasePath);
#else
            return new PhysicalFileProvider(AppContext.BaseDirectory ?? string.Empty);
#endif
        }
    }
}
