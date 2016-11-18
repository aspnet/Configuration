using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Extensions.Configuration.DockerSecrets.Test
{
    public class DockerSecretTests
    {

        [Fact]
        public void ThrowsWhenNotOptionalAndNoSecrets()
        {
            Assert.Throws<FileNotFoundException>(() => new ConfigurationBuilder().AddDockerSecrets().Build());
        }

        [Fact]
        public void DoesNotThrowWhenOptionalAndNoSecrets()
        {
            new ConfigurationBuilder().AddDockerSecrets(optional: true).Build();
        }
    }
}
