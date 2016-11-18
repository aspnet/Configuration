using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public override void Load()
        {
            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if(Source.FileProvider == null)
            {
                if(Source.Optional)
                {
                    return;
                }
                throw new InvalidOperationException($"The FileProvider of {nameof(Source)} is null in a non-optonal provider.");
            }

            var secretsDir = Source.FileProvider.GetDirectoryContents("/");

            if (!secretsDir.Exists && !Source.Optional)
            {
                throw new FileNotFoundException("Secrets directory doesn't exist and is not optional.");
            }

            foreach (var file in secretsDir)
            {
                var stream = file.CreateReadStream();
                try
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        Data.Add(file.Name, streamReader.ReadToEnd());
                    }
                }
                finally
                {
                    stream.Dispose();
                }
            }
        }
    }
}
