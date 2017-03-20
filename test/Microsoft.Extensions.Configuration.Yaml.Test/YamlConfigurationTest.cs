// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.Configuration.Test;
using Xunit;

namespace Microsoft.Extensions.Configuration
{
    public class YamlConfigurationTest
    {
        private YamlConfigurationProvider LoadProvider(string yaml)
        {
            var p = new YamlConfigurationProvider(new YamlConfigurationSource { Optional = true });
            p.Load(TestStreamHelpers.StringToStream(yaml));
            return p;
        }

        [Fact]
        public void LoadKeyValuePairsFromValidYaml()
        {
            var yaml = @"
'firstname': 'test'
'test.last.name': 'last.name'
'residential.address':
 'street.name': 'Something street'
 'zipcode': '12345'
";
            var yamlConfigSrc = LoadProvider(yaml);

            Assert.Equal("test", yamlConfigSrc.Get("firstname"));
            Assert.Equal("last.name", yamlConfigSrc.Get("test.last.name"));
            Assert.Equal("Something street", yamlConfigSrc.Get("residential.address:STREET.name"));
            Assert.Equal("12345", yamlConfigSrc.Get("residential.address:zipcode"));
        }

        [Fact]
        public void LoadMethodCanHandleEmptyValue()
        {
            var yaml = @"
name: ''
";
            var yamlConfigSrc = LoadProvider(yaml);
            Assert.Equal(string.Empty, yamlConfigSrc.Get("name"));
        }

        [Fact]
        public void SupportAndIgnoreComments()
        {
            var yaml = @"
# Comments
name: test
address: # Comments
 street: Something street # Comments
 # Comments
 zipcode: 12345
";
            var yamlConfigSrc = LoadProvider(yaml);
            Assert.Equal("test", yamlConfigSrc.Get("name"));
            Assert.Equal("Something street", yamlConfigSrc.Get("address:street"));
            Assert.Equal("12345", yamlConfigSrc.Get("address:zipcode"));
        }

        [Fact]
        public void ThrowExceptionWhenMissingCurlyBeforeFinishParsing()
        {
            var yaml = @"
{
";

            var exception = Assert.Throws<FormatException>(() => LoadProvider(yaml));
            Assert.Contains("Could not parse the YAML file.", exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenPassingNullAsFilePath()
        {
            var expectedMsg = new ArgumentException(Resources.Error_InvalidFilePath, "path").Message;

            var exception = Assert.Throws<ArgumentException>(() => new ConfigurationBuilder().AddYamlFile(path: null));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenPassingEmptyStringAsFilePath()
        {
            var expectedMsg = new ArgumentException(Resources.Error_InvalidFilePath, "path").Message;

            var exception = Assert.Throws<ArgumentException>(() => new ConfigurationBuilder().AddYamlFile(string.Empty));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void YamlConfiguration_Throws_On_Missing_Configuration_File()
        {
            var config = new ConfigurationBuilder().AddYamlFile("NotExistingConfig.yaml", optional: false);
            var exception = Assert.Throws<FileNotFoundException>(() => config.Build());

            // Assert
            Assert.True(exception.Message.StartsWith($"The configuration file 'NotExistingConfig.yaml' was not found and is not optional. The physical path is '"));
        }

        [Fact]
        public void YamlConfiguration_Does_Not_Throw_On_Optional_Configuration()
        {
            var config = new ConfigurationBuilder().AddYamlFile("NotExistingConfig.yaml", optional: true).Build();
        }

        [Fact]
        public void NotThrowExceptionWhenFileIsEmpty()
        {
            LoadProvider(@"");
        }
    }
}