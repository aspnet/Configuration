// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration.Xml;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for adding <see cref="XmlConfigurationProvider"/>.
    /// </summary>
    public static class XmlConfigurationExtensions
    {
        /// <summary>
        /// Adds the XML configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddXmlFile(this IConfigurationBuilder builder, string path)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Resources.Error_InvalidFilePath, nameof(path));
            }

            return AddXmlFile(builder, source => source.Path = path);
        }

        /// <summary>
        /// Adds a XML configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the <see cref="XmlConfigurationSource"/> to add.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddXmlFile(
            this IConfigurationBuilder builder,
            Action<XmlConfigurationSource> configureSource)
        {
            return builder.AddFileSource(configureSource);
        }
    }
}
