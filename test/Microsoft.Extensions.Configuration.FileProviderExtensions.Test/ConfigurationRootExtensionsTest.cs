// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Microsoft.Extensions.Configuration
{
    public class ConfigurationRootExtensionsTest
    {
        [Fact]
        public void FileConfigurationProviderReloadsOnFileChanged()
        {
            // Arrange
            var tokenSource1 = new CancellationTokenSource();
            var tokenSource2 = new CancellationTokenSource();

            var fileProvider = new MockFileProvider();
            fileProvider.Cancel = tokenSource1;

            var testProvider = new TestFileProvider("ignored") { FileProvider = fileProvider };

            // Act-1
            var configuration = new ConfigurationBuilder().Add(testProvider).Build();

            // Assert-1
            Assert.Equal(0, testProvider.LoadCount);
            Assert.Equal(1, fileProvider.WatchCount);

            // Act-2
            fileProvider.Cancel = tokenSource2;
            tokenSource1.Cancel();

            // Assert-2
            Assert.Equal(1, testProvider.LoadCount);
            Assert.Equal(2, fileProvider.WatchCount);
        }

        private class TestFileProvider : FileConfigurationProvider
        {
            public TestFileProvider(string path) : base(path, optional: false, reloadOnFileChanged: true)
            {
            }

            public int LoadCount { get; set; }

            public override void Load(Stream stream)
            {
                LoadCount++;
            }
        }

        private class MockFileProvider : IFileProvider
        {
            public CancellationTokenSource Cancel { get; set; }

            public int WatchCount { get; private set; }

            public IDirectoryContents GetDirectoryContents(string subpath)
            {
                throw new NotImplementedException();
            }

            public IFileInfo GetFileInfo(string subpath)
            {
                throw new NotImplementedException();
            }

            public IChangeToken Watch(string filter)
            {
                WatchCount++;
                return new CancellationChangeToken(Cancel.Token);
            }
        }
    }
}
