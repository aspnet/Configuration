// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration.Test;
using Xunit;

namespace Microsoft.Extensions.Configuration.CommandLine.Test
{
    public class CommandLineTest
    {
        [Fact]
        public void LoadKeyValuePairsFromCommandLineArgumentsWithoutSwitchMappings()
        {
            var args = new string[]
                {
                    "Key1=Value1",
                    "--Key2=Value2",
                    "/Key3=Value3",
                    "--Key4", "Value4",
                    "/Key5", "Value5"
                };
            var cmdLineConfig = new CommandLineConfigurationProvider(args);

            cmdLineConfig.Load();

            Assert.Equal("Value1", cmdLineConfig.Get("Key1"));
            Assert.Equal("Value2", cmdLineConfig.Get("Key2"));
            Assert.Equal("Value3", cmdLineConfig.Get("Key3"));
            Assert.Equal("Value4", cmdLineConfig.Get("Key4"));
            Assert.Equal("Value5", cmdLineConfig.Get("Key5"));
        }

        [Fact]
        public void LoadKeyValuePairsFromCommandLineArgumentsWithSwitchMappings()
        {
            var args = new string[]
                {
                    "-K1=Value1",
                    "--Key2=Value2",
                    "/Key3=Value3",
                    "--Key4", "Value4",
                    "/Key5", "Value5",
                    "/Key6=Value6"
                };
            var switchMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "-K1", "LongKey1" },
                    { "--Key2", "SuperLongKey2" },
                    { "--Key6", "SuchALongKey6"}
                };
            var cmdLineConfig = new CommandLineConfigurationProvider(args, switchMappings);

            cmdLineConfig.Load();

            Assert.Equal("Value1", cmdLineConfig.Get("LongKey1"));
            Assert.Equal("Value2", cmdLineConfig.Get("SuperLongKey2"));
            Assert.Equal("Value3", cmdLineConfig.Get("Key3"));
            Assert.Equal("Value4", cmdLineConfig.Get("Key4"));
            Assert.Equal("Value5", cmdLineConfig.Get("Key5"));
            Assert.Equal("Value6", cmdLineConfig.Get("SuchALongKey6"));
        }

        [Fact]
        public void ThrowExceptionWhenPassingSwitchMappingsWithDuplicatedKeys()
        {
            // Arrange
            var args = new string[]
                {
                    "-K1=Value1",
                    "--Key2=Value2",
                    "/Key3=Value3",
                    "--Key4", "Value4",
                    "/Key5", "Value5"
                };
            var switchMappings = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    { "--KEY1", "LongKey1" },
                    { "--key1", "SuperLongKey1" },
                    { "-Key2", "LongKey2" },
                    { "-KEY2", "LongKey2"}
                };

            // Find out the duplicate expected be be reported
            var expectedDup = string.Empty;
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var mapping in switchMappings)
            {
                if (set.Contains(mapping.Key))
                {
                    expectedDup = mapping.Key;
                    break;
                }

                set.Add(mapping.Key);
            }

            var expectedMsg = new ArgumentException(Resources.
                FormatError_DuplicatedKeyInSwitchMappings(expectedDup), "switchMappings").Message;

            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => new CommandLineConfigurationProvider(args, switchMappings));

            // Assert
            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenSwitchMappingsContainInvalidKey()
        {
            var args = new string[]
                {
                    "-K1=Value1",
                    "--Key2=Value2",
                    "/Key3=Value3",
                    "--Key4", "Value4",
                    "/Key5", "Value5"
                };
            var switchMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "-K1", "LongKey1" },
                    { "--Key2", "SuperLongKey2" },
                    { "/Key3", "AnotherSuperLongKey3" }
                };
            var expectedMsg = new ArgumentException(Resources.FormatError_InvalidSwitchMapping("/Key3"),
                "switchMappings").Message;

            var exception = Assert.Throws<ArgumentException>(
                () => new CommandLineConfigurationProvider(args, switchMappings));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenNullIsPassedToConstructorAsArgs()
        {
            string[] args = null;
            var expectedMsg = new ArgumentNullException("args").Message;

            var exception = Assert.Throws<ArgumentNullException>(() => new CommandLineConfigurationProvider(args));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void OverrideValueWhenKeyIsDuplicated()
        {
            var args = new string[]
                {
                    "/Key1=Value1",
                    "--Key1=Value2"
                };
            var cmdLineConfig = new CommandLineConfigurationProvider(args);

            cmdLineConfig.Load();

            Assert.Equal("Value2", cmdLineConfig.Get("Key1"));
        }

        [Fact]
        public void ThrowExceptionWhenValueForAKeyIsMissing()
        {
            var args = new string[]
                {
                    "--Key1", "Value1",
                    "/Key2" /* The value for Key2 is missing here */
                };
            var expectedMsg = new FormatException(Resources.FormatError_ValueIsMissing("/Key2")).Message;
            var cmdLineConfig = new CommandLineConfigurationProvider(args);

            var exception = Assert.Throws<FormatException>(() => cmdLineConfig.Load());

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenAnArgumentCannotBeRecognized()
        {
            var args = new string[]
                {
                    "ArgWithoutPrefixAndEqualSign"
                };
            var expectedMsg = new FormatException(
                Resources.FormatError_UnrecognizedArgumentFormat("ArgWithoutPrefixAndEqualSign")).Message;
            var cmdLineConfig = new CommandLineConfigurationProvider(args);

            var exception = Assert.Throws<FormatException>(() => cmdLineConfig.Load());

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenShortSwitchNotDefined()
        {
            var args = new string[]
                {
                    "-Key1", "Value1",
                };
            var switchMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "-Key2", "LongKey2" }
                };
            var expectedMsg = new FormatException(Resources.FormatError_ShortSwitchNotDefined("-Key1")).Message;
            var cmdLineConfig = new CommandLineConfigurationProvider(args, switchMappings);

            var exception = Assert.Throws<FormatException>(() => cmdLineConfig.Load());

            Assert.Equal(expectedMsg, exception.Message);
        }
    }
}
