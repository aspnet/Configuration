// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Framework.Configuration.Helper;
using Xunit;

namespace Microsoft.Framework.Configuration.Test
{
    public class ConfigurationHelperTest
    {
        [Fact]
        public void ResolveFilePath()
        {
            var testFile = Path.GetTempFileName();
            var testDir = Path.GetDirectoryName(testFile);
            var testFileName = Path.GetFileName(testFile);
            var mockSourceRoot = new MockConfigurationBuilder();

            mockSourceRoot.SetBasePath(testDir);
            var actualPath = ConfigurationHelper.ResolveConfigurationFilePath(mockSourceRoot, testFileName);

            Assert.Equal(testFile, actualPath);
        }

        [Fact]
        public void NotThrowWhenFileDoesNotExists()
        {
            var testFile = Path.GetTempFileName();
            var testDir = Path.GetDirectoryName(testFile);
            var testFileName = Path.GetFileName(testFile);
            var mockBuilder = new MockConfigurationBuilder();

            mockBuilder.SetBasePath(testDir);
            File.Delete(testFile);

            var path = ConfigurationHelper.ResolveConfigurationFilePath(mockBuilder, testFileName);

            Assert.Equal(testFile, path);
        }

        private class MockConfigurationBuilder : IConfigurationBuilder
        {
            private Dictionary<string, object> _properties = new Dictionary<string, object>();

            public MockConfigurationBuilder()
            {
                _properties["BasePath"] = string.Empty;
            }

            public string this[string key]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public Dictionary<string,object> Properties
            {
                get
                {
                    return _properties;
                }
                set
                {
                    _properties = value;
                }
            }

            public IEnumerable<IConfigurationSource> Sources
            {
                get { throw new NotImplementedException(); }
            }

            public IConfigurationBuilder Add(IConfigurationSource configurationSource)
            {
                throw new NotImplementedException();
            }

            public IConfiguration GetSection(string key)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<KeyValuePair<string, IConfiguration>> GetChildren()
            {
                throw new NotImplementedException();
            }

            public void Reload()
            {
                throw new NotImplementedException();
            }

            public IConfigurationRoot Build()
            {
                throw new NotImplementedException();
            }
        }
    }
}