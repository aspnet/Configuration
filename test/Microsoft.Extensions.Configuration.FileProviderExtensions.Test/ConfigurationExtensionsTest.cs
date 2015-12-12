﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNet.FileProviders;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Microsoft.Extensions.Configuration
{
    public class ConfigurationExtensionsTest
    {
        [Fact]
        public void ReloadOnChanged_GetTokenBeforeReload()
        {
            // Arrange
            var tokenSource1 = new CancellationTokenSource();
            var tokenSource2 = new CancellationTokenSource();

            var fileProvider = new MockFileProvider();
            fileProvider.Cancel = tokenSource1;

            var configuration = new MockConfigurationRoot();
            configuration.OnReload = () => Assert.Equal(2, fileProvider.WatchCount);

            // Act-1
            configuration.ReloadOnChanged(fileProvider, "config.json");

            // Assert-1
            Assert.Equal(1, fileProvider.WatchCount);
            Assert.Equal(0, configuration.ReloadCount);

            // Act-2
            fileProvider.Cancel = tokenSource2;
            tokenSource1.Cancel();

            Assert.Equal(2, fileProvider.WatchCount);
            Assert.Equal(1, configuration.ReloadCount);
        }

        [Fact]
        public void ReloadTokensFireOnce()
        {
            var count = 0;
            var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var cleanup = config.GetReloadToken().RegisterChangeCallback(s => count++, this);
            Assert.Equal(0, count);
            config.ReloadAll();
            Assert.Equal(1, count);
            config.ReloadAll();
            Assert.Equal(1, count);
        }

        [Fact]
        public void ReloadTokensNeedsToBeReregisteredToFireAgain()
        {
            var count = 0;
            var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var cleanup = config.GetReloadToken().RegisterChangeCallback(s => count++, this);
            Assert.Equal(0, count);
            config.ReloadAll();
            Assert.Equal(1, count);
            cleanup = config.GetReloadToken().RegisterChangeCallback(s => count++, this);
            config.ReloadAll();
            Assert.Equal(2, count);
        }

        [Fact]
        public void ReloadTokensDoNotFireAfterDispose()
        {
            var count = 0;
            var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var cleanup = config.GetReloadToken().RegisterChangeCallback(s => count++, this);
            Assert.Equal(0, count);
            cleanup.Dispose();
            config.ReloadAll();
            Assert.Equal(0, count);
        }

        private class MockConfigurationRoot : IConfiguration
        {
            public Action OnReload { get; set; }

            public int ReloadCount { get; private set; }

            public string this[string key]
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }


            public IEnumerable<IConfigurationSection> GetChildren()
            {
                throw new NotImplementedException();
            }
            public IChangeToken GetReloadToken()
            {
                throw new NotImplementedException();
            }

            public IConfigurationSection GetSection(string key)
            {
                throw new NotImplementedException();
            }

            public void ReloadAll()
            {
                OnReload?.Invoke();
                ReloadCount++;
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
