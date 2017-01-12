using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration.DockerSecrets
{
    /// <summary>
    /// An <see cref="ConfigurationProvider"/> for docker secrets.
    /// </summary>
    public class DockerSecretsConfigurationSource : IConfigurationSource
    {
        //TODO: I don't know that this is configurable, maybe should just be a const.
        /// <summary>
        /// The secrets directory which will be used if FileProvider is not set.
        /// </summary>
        public string SecretsDirectory { get; set; } = "/run/secrets";

        /// <summary>
        /// The FileProvider representing the secrets directory.
        /// </summary>
        public IFileProvider FileProvider { get; set; }

        /// <summary>
        /// Docker secrets that start with this prefix will be excluded.
        /// </summary>
        public string IgnorePrefx { get; set; } = "ignore.";

        /// <summary>
        /// If false, will throw if the secrets directory doesn't exist.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Builds the <see cref="DockerSecretsConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="DockerSecretsConfigurationProvider"/></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DockerSecretsConfigurationProvider(this);
        }
    }
}
