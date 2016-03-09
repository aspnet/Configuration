// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration.Ini;

namespace Microsoft.Extensions.Configuration
{
    public static class IniConfigurationExtensions
    {
        /// <summary>
        /// Adds the INI configuration provider at <paramref name="path"/> to <paramref name="configurationBuilder"/>.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="configurationBuilder"/>.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddIniFile(this IConfigurationBuilder configurationBuilder, string path)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Resources.Error_InvalidFilePath, nameof(path));
            }

            return AddIniFile(configurationBuilder, source => source.Path = path);
        }

        /// <summary>
        /// Adds a INI configuration source to <paramref name="configurationBuilder"/>.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the <see cref="IniConfigurationSource"/> to add.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddIniFile(
            this IConfigurationBuilder configurationBuilder,
            Action<IniConfigurationSource> configureSource)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            if (configureSource == null)
            {
                throw new ArgumentNullException(nameof(configureSource));
            }

            var source = new IniConfigurationSource();
            source.FileProvider = configurationBuilder.GetFileProvider();
            configureSource(source);
            configurationBuilder.Add(source);
            return configurationBuilder;
        }
    }
}
