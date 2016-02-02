// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Extensions.Configuration.Json;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for adding <see cref="JsonConfigurationProvider"/>.
    /// </summary>
    public static class JsonConfigurationExtensions
    {
        /// <summary>
        /// Adds the JSON configuration provider at <paramref name="path"/> to <paramref name="configurationBuilder"/>.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Absolute path or path relative to <see cref="IConfigurationBuilder.BasePath"/> of
        /// <paramref name="configurationBuilder"/>.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddJsonFile(
            this IConfigurationBuilder configurationBuilder,
            string path)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            return AddJsonFile(configurationBuilder, path, optional: false, reloadOnFileChanged: false);
        }

        /// <summary>
        /// Adds the JSON configuration provider at <paramref name="path"/> to <paramref name="configurationBuilder"/>.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Absolute path or path relative to <see cref="IConfigurationBuilder.BasePath"/> of
        /// <paramref name="configurationBuilder"/>.</param>
        /// <param name="optional">Determines if loading the configuration provider is optional.</param>
        /// <param name="reloadOnFileChanged">Determines if the configuration provider should be reloaded automatically when the file changes.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        /// <exception cref="ArgumentException">If <paramref name="path"/> is null or empty.</exception>
        /// <exception cref="FileNotFoundException">If <paramref name="optional"/> is <c>false</c> and the file cannot
        /// be resolved.</exception>
        public static IConfigurationBuilder AddJsonFile(
            this IConfigurationBuilder configurationBuilder,
            string path,
            bool optional,
            bool reloadOnFileChanged)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Resources.Error_InvalidFilePath, nameof(path));
            }

            var fullPath = Path.Combine(configurationBuilder.GetBasePath(), path);

            if (!optional && !File.Exists(fullPath))
            {
                throw new FileNotFoundException(Resources.FormatError_FileNotFound(fullPath), fullPath);
            }

            configurationBuilder.Add(new JsonConfigurationProvider(fullPath, optional: optional, reloadOnFileChanged: reloadOnFileChanged));
            return configurationBuilder;
        }
    }
}
