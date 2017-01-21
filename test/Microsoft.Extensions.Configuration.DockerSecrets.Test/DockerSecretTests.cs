using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Text;

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

        [Fact]
        public void CanLoadMultipleSecrets()
        {
            var testFileProvider = new TestFileProvider(
                new TestFile("Secret1", "SecretValue1"),
                new TestFile("Secret2", "SecretValue2"));

            var source = new DockerSecretsConfigurationSource(testFileProvider);

            var config = new ConfigurationBuilder()
                .Add(source)
                .Build();

            Assert.Equal("SecretValue1", config["Secret1"]);
            Assert.Equal("SecretValue2", config["Secret2"]);
        }
    }

    class TestFileProvider : IFileProvider
    {
        IDirectoryContents _contents;
        
        public TestFileProvider(params IFileInfo[] files)
        {
            _contents = new TestDirectoryContents(files);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return _contents;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            throw new NotImplementedException();
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }

    class TestDirectoryContents : IDirectoryContents
    {
        List<IFileInfo> _list;

        public TestDirectoryContents(params IFileInfo[] files)
        {
            _list = new List<IFileInfo>(files);
        }

        public bool Exists
        {
            get
            {
                return true;
            }
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class TestFile : IFileInfo
    {
        private string _name;
        private string _contents;

        public bool Exists
        {
            get
            {
                return true;
            }
        }

        public bool IsDirectory
        {
            get
            {
                return false;
            }
        }

        public DateTimeOffset LastModified
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string PhysicalPath
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TestFile(string name, string contents)
        {
            _name = name;
            _contents = contents;
        }

        public Stream CreateReadStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_contents));
        }
    }
}