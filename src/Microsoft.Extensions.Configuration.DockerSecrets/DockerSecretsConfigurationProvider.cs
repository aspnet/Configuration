using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration.DockerSecrets
{
    public class DockerSecretsConfigurationProvider : ConfigurationProvider
    {
        DockerSecretsConfigurationSource Source { get; set; }

        public DockerSecretsConfigurationProvider(DockerSecretsConfigurationSource source)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Source = source;
        }

        public static Dictionary<string, string> ReadDockerSecrets(IFileProvider fileProvider)
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var secretsDir = fileProvider.GetDirectoryContents("/");

            //if (!secretsDir.Exists && !Source.Optional)
            //{
            //    throw new FileNotFoundException("Secrets directory doesn't exist and is not optional.");
            //}

            foreach (var file in secretsDir)
            {
                if (file.IsDirectory)
                {
                    continue;
                }

                using (var stream = file.CreateReadStream())
                using (var streamReader = new StreamReader(stream))
                {
                    data.Add(file.Name, streamReader.ReadToEnd());
                }
            }
            return data;
        }

        public override void Load()
        {
            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if(Source.FileProvider == null)
            {
                if(Source.Optional)
                {
                    return;
                }
                throw new InvalidOperationException($"The FileProvider of {nameof(Source)} is null in a non-optional provider.");
            }

            var secretsDir = Source.FileProvider.GetDirectoryContents("/");

            if (!secretsDir.Exists && !Source.Optional)
            {
                throw new FileNotFoundException("Secrets directory doesn't exist and is not optional.");
            }

            Data = ReadDockerSecrets(Source.FileProvider);
        }
    }
}
