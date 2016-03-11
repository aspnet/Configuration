// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration
{
    public static class FileConfigurationExtensions
    {
        private static string FileSourceDefaultsKey = "FileSourceDefaults";

        private class DefaultFileSource : FileConfigurationSource
        {
            public override IConfigurationProvider Build(IConfigurationBuilder builder)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sets the default <see cref="IFileProvider"/> to be used for file-based providers.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="fileProvider">The default file provider instance.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder SetFileProvider(this IConfigurationBuilder builder, IFileProvider fileProvider)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (fileProvider == null)
            {
                throw new ArgumentNullException(nameof(fileProvider));
            }

            builder.GetFileSourceDefaults().FileProvider = fileProvider;
            return builder;
        }

        /// <summary>
        /// Configures the default <see cref="FileConfigurationSource"/> settings to be used for file-based providers.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureDefaults">Configures the default <see cref="FileConfigurationSource"/>.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder ConfigureFileSourceDefaults(this IConfigurationBuilder builder, Action<FileConfigurationSource> configureDefaults)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureDefaults == null)
            {
                throw new ArgumentNullException(nameof(configureDefaults));
            }

            configureDefaults(builder.GetFileSourceDefaults());
            return builder;
        }

        public static FileConfigurationSource GetFileSourceDefaults(this IConfigurationBuilder configurationBuilder)
        {
            object defaults;
            if (configurationBuilder.Properties.TryGetValue(FileSourceDefaultsKey, out defaults))
            {
                return (FileConfigurationSource)defaults;
            }
            var newDefaults = new DefaultFileSource();
            configurationBuilder.Properties[FileSourceDefaultsKey] = newDefaults;
            return newDefaults;

        }

        /// <summary>
        /// Sets the FileProvider for file-based providers to a PhysicalFileProvider with the base path.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="basePath">The absolute path of file-based providers.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder SetFileProvider(this IConfigurationBuilder builder, string basePath)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            return builder.SetFileProvider(new PhysicalFileProvider(basePath));
        }

        /// <summary>
        /// Adds a file configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the file source.</typeparam>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the <see cref="IConfigurationSource"/> to add.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddFileSource<TSource>(this IConfigurationBuilder builder, Action<TSource> configureSource)
            where TSource : IConfigurationSource, new()
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureSource == null)
            {
                throw new ArgumentNullException(nameof(configureSource));
            }

            var source = new TSource();
            configureSource(source);
            builder.Add(source);
            return builder;
        }
    }
}
