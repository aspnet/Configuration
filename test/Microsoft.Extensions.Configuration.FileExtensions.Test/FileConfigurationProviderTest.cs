// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration.FileProviders;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Microsoft.Extensions.Configuration
{
    public class FileConfigurationProviderTest
    {
        [Fact]
        public void FileConfigurationProviderReloadsOnFileChanged()
        {
            // Arrange
            var tokenSource1 = new CancellationTokenSource();
            var tokenSource2 = new CancellationTokenSource();

            var fileProvider = new MockFileProvider();
            fileProvider.Cancel = tokenSource1;

            var testProvider = new TestFileProvider("ignored");
            testProvider.ReloadWhenFileChanges(fileProvider);

            // Act-1
            var configuration = new ConfigurationBuilder().Add(testProvider).Build();

            // Assert-1
            Assert.Equal(1, testProvider.LoadCount);
            Assert.Equal(1, fileProvider.WatchCount);

            // Act-2
            fileProvider.Cancel = tokenSource2;
            tokenSource1.Cancel();

            // Assert-2
            Assert.Equal(2, testProvider.LoadCount);
            Assert.Equal(2, fileProvider.WatchCount);
        }

        private class TestFileProvider : FileConfigurationProvider
        {
            public TestFileProvider(string path) : base(path, optional: false)
            {
            }

            public int LoadCount { get; set; }

            public override void Load() => LoadCount++;

            public override void Load(Stream stream)
            {
            }
        }

        private class MockFileInfo : IFileInfo
        {
            public bool Exists => true;

            public bool IsDirectory => false;

            public DateTimeOffset LastModified => DateTimeOffset.Now;

            public long Length => 0;

            public string Name
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string PhysicalPath
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public Stream CreateReadStream() => new MemoryStream();
        }

        private class MockFileProvider : IFileProvider
        {
            public CancellationTokenSource Cancel { get; set; }

            public int WatchCount { get; private set; }

            public IDirectoryContents GetDirectoryContents(string subpath)
            {
                throw new NotImplementedException();
            }

            public IFileInfo GetFileInfo(string subpath) => new MockFileInfo();

            public IChangeToken Watch(string filter)
            {
                WatchCount++;
                return new CancellationChangeToken(Cancel.Token);
            }
        }
    }
}
