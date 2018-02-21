using System;
using Microsoft.Extensions.Configuration.DirectoryFiles;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for registering <see cref="DirectoryFilesConfigurationProvider"/> with <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class DirectoryFilesConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds configuration using files from a directory. File names are used as the key,
        /// file contents are used as the value.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="directoryPath">The path to the directory.</param>
        /// <param name="optional">Whether the directory is optional.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddDirectoryFiles(this IConfigurationBuilder builder, string directoryPath, bool optional)
            => builder.AddDirectoryFiles(source =>
            {
                source.FileProvider = new PhysicalFileProvider(directoryPath);
                source.Optional = optional;
            });

        /// <summary>
        /// Adds configuration using files from a directory. File names are used as the key,
        /// file contents are used as the value.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddDirectoryFiles(this IConfigurationBuilder builder, Action<DirectoryFilesConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}
