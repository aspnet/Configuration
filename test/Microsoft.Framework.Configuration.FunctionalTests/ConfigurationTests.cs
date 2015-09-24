// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Microsoft.Framework.Configuration.Test
{
    public class ConfigurationTests : IDisposable
    {
        private string _iniConfigFilePath;
        private string _xmlConfigFilePath;
        private string _jsonConfigFilePath;
        private static readonly string _iniConfigFileContent =
            @"IniKey1=IniValue1
[IniKey2]
# Comments
IniKey3=IniValue2
; Comments
IniKey4=IniValue3
IniKey5:IniKey6=IniValue4
/Comments
[CommonKey1:CommonKey2]
IniKey7=IniValue5
CommonKey3:CommonKey4=IniValue6";
        private static readonly string _xmlConfigFileContent =
            @"<settings XmlKey1=""XmlValue1"">
  <!-- Comments -->
  <XmlKey2 XmlKey3=""XmlValue2"">
    <!-- Comments -->
    <XmlKey4>XmlValue3</XmlKey4>
    <XmlKey5 Name=""XmlKey6"">XmlValue4</XmlKey5>
  </XmlKey2>
  <CommonKey1 Name=""CommonKey2"" XmlKey7=""XmlValue5"">
    <!-- Comments -->
    <CommonKey3 CommonKey4=""XmlValue6"" />
  </CommonKey1>
</settings>";
        private static readonly string _jsonConfigFileContent =
            @"{
  ""JsonKey1"": ""JsonValue1"",
  ""Json.Key2"": {
    ""JsonKey3"": ""JsonValue2"",
    ""Json.Key4"": ""JsonValue3"",
    ""JsonKey5:JsonKey6"": ""JsonValue4""
  },
  ""CommonKey1"": {
    ""CommonKey2"": {
      ""JsonKey7"": ""JsonValue5"",
      ""CommonKey3:CommonKey4"": ""JsonValue6""
    }
  }
}";
        private static readonly Dictionary<string, string> _memConfigContent = new Dictionary<string, string>
            {
                { "MemKey1", "MemValue1" },
                { "MemKey2:MemKey3", "MemValue2" },
                { "MemKey2:MemKey4", "MemValue3" },
                { "MemKey2:MemKey5:MemKey6", "MemValue4" },
                { "CommonKey1:CommonKey2:MemKey7", "MemValue5" },
                { "CommonKey1:CommonKey2:CommonKey3:CommonKey4", "MemValue6" }
            };

        public ConfigurationTests()
        {
            _iniConfigFilePath = Path.GetTempFileName();
            _xmlConfigFilePath = Path.GetTempFileName();
            _jsonConfigFilePath = Path.GetTempFileName();

            File.WriteAllText(_iniConfigFilePath, _iniConfigFileContent);
            File.WriteAllText(_xmlConfigFilePath, _xmlConfigFileContent);
            File.WriteAllText(_jsonConfigFilePath, _jsonConfigFileContent);
        }

        [Fact]
        public void LoadAndCombineKeyValuePairsFromDifferentConfigurationSources()
        {
            // Arrange
            var builder = new ConfigurationBuilder();

            // Act
            builder.AddIniFile(_iniConfigFilePath);
            builder.AddJsonFile(_jsonConfigFilePath);
            builder.AddXmlFile(_xmlConfigFilePath);
            builder.AddInMemoryCollection(_memConfigContent);

            var config = builder.Build();

            // Assert
            Assert.Equal("IniValue1", config["IniKey1"]);
            Assert.Equal("IniValue2", config["IniKey2:IniKey3"]);
            Assert.Equal("IniValue3", config["IniKey2:IniKey4"]);
            Assert.Equal("IniValue4", config["IniKey2:IniKey5:IniKey6"]);
            Assert.Equal("IniValue5", config["CommonKey1:CommonKey2:IniKey7"]);

            Assert.Equal("JsonValue1", config["JsonKey1"]);
            Assert.Equal("JsonValue2", config["Json.Key2:JsonKey3"]);
            Assert.Equal("JsonValue3", config["Json.Key2:Json.Key4"]);
            Assert.Equal("JsonValue4", config["Json.Key2:JsonKey5:JsonKey6"]);
            Assert.Equal("JsonValue5", config["CommonKey1:CommonKey2:JsonKey7"]);

            Assert.Equal("XmlValue1", config["XmlKey1"]);
            Assert.Equal("XmlValue2", config["XmlKey2:XmlKey3"]);
            Assert.Equal("XmlValue3", config["XmlKey2:XmlKey4"]);
            Assert.Equal("XmlValue4", config["XmlKey2:XmlKey5:XmlKey6"]);
            Assert.Equal("XmlValue5", config["CommonKey1:CommonKey2:XmlKey7"]);

            Assert.Equal("MemValue1", config["MemKey1"]);
            Assert.Equal("MemValue2", config["MemKey2:MemKey3"]);
            Assert.Equal("MemValue3", config["MemKey2:MemKey4"]);
            Assert.Equal("MemValue4", config["MemKey2:MemKey5:MemKey6"]);
            Assert.Equal("MemValue5", config["CommonKey1:CommonKey2:MemKey7"]);

            Assert.Equal("MemValue6", config["CommonKey1:CommonKey2:CommonKey3:CommonKey4"]);
        }

        [Fact]
        public void CanOverrideValuesWithNewConfigurationSource()
        {
            // Arrange
            var builder = new ConfigurationBuilder();

            // Act & Assert
            builder.AddIniFile(_iniConfigFilePath);
            var config = builder.Build();
            Assert.Equal("IniValue6", config["CommonKey1:CommonKey2:CommonKey3:CommonKey4"]);

            builder.AddJsonFile(_jsonConfigFilePath);
            config = builder.Build();
            Assert.Equal("JsonValue6", config["CommonKey1:CommonKey2:CommonKey3:CommonKey4"]);

            builder.AddXmlFile(_xmlConfigFilePath);
            config = builder.Build();
            Assert.Equal("XmlValue6", config["CommonKey1:CommonKey2:CommonKey3:CommonKey4"]);

            builder.AddInMemoryCollection(_memConfigContent);
            config = builder.Build();
            Assert.Equal("MemValue6", config["CommonKey1:CommonKey2:CommonKey3:CommonKey4"]);
        }

        [Fact]
        public void CanSetValuesAndReloadValues()
        {
            // Arrange
            var builder = new ConfigurationBuilder();
            builder.AddIniFile(_iniConfigFilePath);
            builder.AddJsonFile(_jsonConfigFilePath);
            builder.AddXmlFile(_xmlConfigFilePath);

            var config = builder.Build();

            // Act & Assert
            // Set value
            config["CommonKey1:CommonKey2:CommonKey3:CommonKey4"] = "NewValue";

            // All config sources must be updated
            foreach (var src in builder.Sources)
            {
                Assert.Equal("NewValue",
                    (src as ConfigurationSource).Get("CommonKey1:CommonKey2:CommonKey3:CommonKey4"));
            }

            // Recover values by reloading
            config.Reload();

            Assert.Equal("XmlValue6", config["CommonKey1:CommonKey2:CommonKey3:CommonKey4"]);

            // Set value with indexer
            config["CommonKey1:CommonKey2:CommonKey3:CommonKey4"] = "NewValue";

            // All config sources must be updated
            foreach (var src in builder.Sources)
            {
                Assert.Equal("NewValue",
                    (src as ConfigurationSource).Get("CommonKey1:CommonKey2:CommonKey3:CommonKey4"));
            }

            // Recover values by reloading
            config.Reload();
            Assert.Equal("XmlValue6", config["CommonKey1:CommonKey2:CommonKey3:CommonKey4"]);
        }

        [Fact]
        public void LoadIncorrectJsonFile_ThrowFormatException()
        {
            // Arrange
            var json = @"{
                'name': 'test',
                'address': {
                    'street': 'Something street' /*Missing comma*/
                    'zipcode': '12345'
                }
            }";
            var jsonFile = Path.GetTempFileName();
            File.WriteAllText(jsonFile, json);

            var builder = new ConfigurationBuilder();

            // Act & Assert
            var exception = Assert.Throws<FormatException>(() => builder.AddJsonFile(jsonFile));
            Assert.NotNull(exception.Message);

            File.Delete(jsonFile);
        }

        [Fact]
        public void SetBasePathCalledMultipleTimesForEachSource()
        {
            // Arrange
            var builder = new ConfigurationBuilder();
            _jsonConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), "test.json");
            File.WriteAllText(_jsonConfigFilePath, _jsonConfigFileContent);

            // Act
            builder.SetBasePath(Path.GetDirectoryName(_xmlConfigFilePath))
                .AddXmlFile(Path.GetFileName(_xmlConfigFilePath))
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("test.json");

            var config = builder.Build();

            // Assert
            Assert.Equal("JsonValue1", config["JsonKey1"]);
            Assert.Equal("JsonValue2", config["Json.Key2:JsonKey3"]);
            Assert.Equal("JsonValue3", config["Json.Key2:Json.Key4"]);
            Assert.Equal("JsonValue4", config["Json.Key2:JsonKey5:JsonKey6"]);
            Assert.Equal("JsonValue5", config["CommonKey1:CommonKey2:JsonKey7"]);

            Assert.Equal("XmlValue1", config["XmlKey1"]);
            Assert.Equal("XmlValue2", config["XmlKey2:XmlKey3"]);
            Assert.Equal("XmlValue3", config["XmlKey2:XmlKey4"]);
            Assert.Equal("XmlValue4", config["XmlKey2:XmlKey5:XmlKey6"]);

            File.Delete(_jsonConfigFilePath);
        }

        public void Dispose()
        {
            File.Delete(_iniConfigFilePath);
            File.Delete(_xmlConfigFilePath);
            File.Delete(_jsonConfigFilePath);
        }
    }
}
