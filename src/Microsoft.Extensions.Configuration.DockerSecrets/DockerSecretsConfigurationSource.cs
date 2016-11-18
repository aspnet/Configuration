using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration.DockerSecrets
{
    public class DockerSecretsConfigurationSource : IConfigurationSource
    {
        //TODO: I don't know that this is configurable, maybe should just be a const.
        public string SecretsDirectory { get; set; } = "/run/secrets";
        public IFileProvider FileProvider { get; set; }
        public bool Optional { get; set; }


        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DockerSecretsConfigurationProvider(this);
        }

        /// <summary>
        /// </summary>
        public void ResolveFileProvider()
        {
            if (FileProvider == null)
            {
                if (Directory.Exists(SecretsDirectory))
                {
                    FileProvider = new PhysicalFileProvider(SecretsDirectory);
                }
                else if (!Optional)
                {
                    throw new FileNotFoundException("Docker secrets directory does not exist and is not optional.");
                }
            }
        }
    }
}
