// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
// WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF
// TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR
// NON-INFRINGEMENT.
// See the Apache 2 License for the specific language governing
// permissions and limitations under the License.

using System;
using System.IO;
using Microsoft.AspNet.ConfigurationModel.Sources;
using Xunit;

using Resources = Microsoft.AspNet.ConfigurationModel.Json.Resources;

namespace Microsoft.AspNet.ConfigurationModel.Sources
{
    public class JsonConfigurationSourceTest
    {
        private static readonly string ArbitraryFilePath = "Unit tests do not touch file system";

        [Fact]
        public void LoadKeyValuePairsFromValidJson()
        {
            var json = @"
{
    'name': 'test',
    'address': {
        'street': 'Something street',
        'zipcode': '12345'
    }
}";
            var jsonConfigSrc = new JsonConfigurationSource(ArbitraryFilePath);

            jsonConfigSrc.Load(StringToStream(json));

            Assert.Equal(3, jsonConfigSrc.Data.Count);
            Assert.Equal("test", jsonConfigSrc.Data["NAME"]);
            Assert.Equal("Something street", jsonConfigSrc.Data["address:STREET"]);
            Assert.Equal("12345", jsonConfigSrc.Data["address:zipcode"]);
        }

        [Fact]
        public void NonObjectRootIsInvalid()
        {
            var json = @"'test'";
            var jsonConfigSource = new JsonConfigurationSource(ArbitraryFilePath);
            var expectedMsg = Resources.FormatError_RootMustBeAnObject(string.Empty, 1, 6);

            var exception = Assert.Throws<FormatException>(() => jsonConfigSource.Load(StringToStream(json)));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void SupportAndIgnoreComments()
        {
            var json = @"/* Comments */
                {/* Comments */
                ""name"": /* Comments */ ""test"",
                ""address"": {
                    ""street"": ""Something street"", /* Comments */
                    ""zipcode"": ""12345""
                }
            }";
            var jsonConfigSrc = new JsonConfigurationSource(ArbitraryFilePath);

            jsonConfigSrc.Load(StringToStream(json));

            Assert.Equal(3, jsonConfigSrc.Data.Count);
            Assert.Equal("test", jsonConfigSrc.Data["name"]);
            Assert.Equal("Something street", jsonConfigSrc.Data["address:street"]);
            Assert.Equal("12345", jsonConfigSrc.Data["address:zipcode"]);
        }

        [Fact]
        public void ArraysAreNotSupported()
        {
            var json = @"{
                'name': 'test',
                'address': ['Something street', '12345']
            }";
            var jsonConfigSource = new JsonConfigurationSource(ArbitraryFilePath);
            var expectedMsg = Resources.FormatError_UnsupportedJSONToken("StartArray", "address", 3, 29);

            var exception = Assert.Throws<FormatException>(() => jsonConfigSource.Load(StringToStream(json)));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenUnexpectedEndFoundBeforeFinishParsing()
        {
            var json = @"{
                'name': 'test',
                'address': {
                    'street': 'Something street',
                    'zipcode': '12345'
                }
            /* Missing a right brace here*/";
            var jsonConfigSource = new JsonConfigurationSource(ArbitraryFilePath);
            var expectedMsg = Resources.FormatError_UnexpectedEnd("address", 7, 44);

            var exception = Assert.Throws<FormatException>(() => jsonConfigSource.Load(StringToStream(json)));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenPassingNullAsFilePath()
        {
            var expectedMsg = new ArgumentException(Resources.Error_InvalidFilePath, "path").Message;

            var exception = Assert.Throws<ArgumentException>(() => new JsonConfigurationSource(null));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenPassingEmptyStringAsFilePath()
        {
            var expectedMsg = new ArgumentException(Resources.Error_InvalidFilePath, "path").Message;

            var exception = Assert.Throws<ArgumentException>(() => new JsonConfigurationSource(string.Empty));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenKeyIsDuplicated()
        {
            var json = @"{
                'name': 'test',
                'address': {
                    'street': 'Something street',
                    'zipcode': '12345'
                },
                'name': 'new name'
            }";
            var jsonConfigSrc = new JsonConfigurationSource(ArbitraryFilePath);

            var exception = Assert.Throws<FormatException>(() => jsonConfigSrc.Load(StringToStream(json)));

            Assert.Equal(Resources.FormatError_KeyIsDuplicated("name"), exception.Message);
        }

        [Fact]
        public void CommitMethodPreservesCommments()
        {
            var json = @"{
  ""name"": ""test"",
  ""address"": {
    ""street"": ""Something street"",
    ""zipcode"": ""12345""
  }
}";
            var jsonConfigSrc = new JsonConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            jsonConfigSrc.Load(StringToStream(json));

            jsonConfigSrc.Commit(StringToStream(json), outputCacheStream);

            var newContents = StreamToString(outputCacheStream);
            Assert.Equal(json, newContents);
        }

        [Fact]
        public void CommitMethodUpdatesValues()
        {
            var json = @"{
  ""name"": ""test"",
  ""address"": {
    ""street"": ""Something street"",
    ""zipcode"": ""12345""
  }
}";
            var jsonConfigSrc = new JsonConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            jsonConfigSrc.Load(StringToStream(json));
            jsonConfigSrc.Set("name", "new_name");
            jsonConfigSrc.Set("address:zipcode", "67890");

            jsonConfigSrc.Commit(StringToStream(json), outputCacheStream);

            var newContents = StreamToString(outputCacheStream);
            Assert.Equal(json.Replace("test", "new_name").Replace("12345", "67890"), newContents);
        }

        [Fact]
        public void CommitOperationThrowsExceptionWhenFindInvalidModificationAfterLoadOperation()
        {
            var json = @"{
  ""name"": ""test"",
  ""address"": {
    ""street"": ""Something street"",
    ""zipcode"": ""12345""
  }
}";
            var modifiedJson = @"
{
  ""name"": [""first"", ""last""],
  ""address"": {
    ""street"": ""Something street"",
    ""zipcode"": ""12345""
  }
}";
            var jsonConfigSrc = new JsonConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            jsonConfigSrc.Load(StringToStream(json));

            var exception = Assert.Throws<FormatException>(
                () => jsonConfigSrc.Commit(StringToStream(modifiedJson), outputCacheStream));

            Assert.Equal(Resources.FormatError_UnsupportedJSONToken("StartArray", "name", 3, 12), exception.Message);
        }

        [Fact]
        public void CommitOperationThrowsExceptionWhenFindNewlyAddedKeyAfterLoadOperation()
        {
            var json = @"{
  ""name"": ""test"",
  ""address"": {
    ""street"": ""Something street"",
    ""zipcode"": ""12345""
  }
}";
            var newJson = @"{
  ""name"": ""test"",
  ""address"": {
    ""street"": ""Something street"",
    ""zipcode"": ""12345""
  },
  ""NewKey"": ""NewValue""
}";
            var jsonConfigSrc = new JsonConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            jsonConfigSrc.Load(StringToStream(json));

            var exception = Assert.Throws<InvalidOperationException>(
                () => jsonConfigSrc.Commit(StringToStream(newJson), outputCacheStream));

            Assert.Equal(Resources.FormatError_CommitWhenNewKeyFound("NewKey"), exception.Message);
        }

        [Fact]
        public void CommitOperationThrowsExceptionWhenKeysAreMissingInConfigFile()
        {
            var json = @"{
  ""name"": ""test"",
  ""address"": {
    ""street"": ""Something street"",
    ""zipcode"": ""12345""
  }
}";
            var jsonConfigSrc = new JsonConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            jsonConfigSrc.Load(StringToStream(json));
            json = json.Replace(@"""name"": ""test"",", string.Empty);

            var exception = Assert.Throws<InvalidOperationException>(
                () => jsonConfigSrc.Commit(StringToStream(json), outputCacheStream));

            Assert.Equal(Resources.FormatError_CommitWhenKeyMissing("name"), exception.Message);
        }

        [Fact]
        public void CanCreateNewConfig()
        {
            var targetJson = @"{
  ""name"": ""test"",
  ""address:street"": ""Something street"",
  ""address:zipcode"": ""12345""
}";
            var jsonConfigSrc = new JsonConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            jsonConfigSrc.Data["name"] = "test";
            jsonConfigSrc.Data["address:street"] = "Something street";
            jsonConfigSrc.Data["address:zipcode"] = "12345";

            jsonConfigSrc.GenerateNewConfig(outputCacheStream);

            Assert.Equal(targetJson, StreamToString(outputCacheStream));
        }

        private static Stream StringToStream(string str)
        {
            var memStream = new MemoryStream();
            var textWriter = new StreamWriter(memStream);
            textWriter.Write(str);
            textWriter.Flush();
            memStream.Seek(0, SeekOrigin.Begin);

            return memStream;
        }

        private static string StreamToString(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
