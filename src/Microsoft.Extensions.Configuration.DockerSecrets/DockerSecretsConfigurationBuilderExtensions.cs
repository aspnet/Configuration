using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.DockerSecrets;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration
{
    public static class DockerSecretsConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder)
        {
            return AddDockerSecrets(builder, false, null);
        }

        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder, bool optional)
        {
            return AddDockerSecrets(builder, optional, null);
        }

        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder, bool optional, IFileProvider provider)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var source = new DockerSecretsConfigurationSource
            {
                FileProvider = provider,
                Optional = optional
            };

            builder.Add(source);
            return builder;
        }
    }
}
