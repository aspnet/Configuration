// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Xunit;

namespace Microsoft.Extensions.Configuration.Test
{
    /// <summary>
    /// Base class for tests that exercise basic functionality that all configuration providers should support.
    /// </summary>
    public abstract class ConfigurationSpecificationTestBase
    {

        /// <summary>
        /// Tests will use this method to specify configuration data.
        /// </summary>
        /// <param name="configData"></param>
        /// <returns></returns>
        public abstract IConfigurationBuilder LoadTestData(Dictionary<string, string> configData);

        /// <summary>
        /// Test
        /// </summary>
        [Fact]
        public void KeysShouldBeCaseInsensitive()
        {
            var data = new Dictionary<string, string>()
            {
                {"Root", "Root"},
                {"Nested1:KEY", "Nested1"},
                {"Nested1:key2", "Nested1-2"},
                {"Nested1:Nested2:Key", "Nested2"}
            };
            var config = LoadTestData(data).Build();

            Assert.Equal("Root", config["ROOT"]);
            Assert.Equal(config["Root"], config["ROOT"]);
            Assert.Equal("Nested1", config.GetSection("NESTED1")["kEY"]);
            Assert.Equal("Nested1-2", config.GetSection("NesteD1")["KEY2"]);
            Assert.Equal("Nested2", config.GetSection("NESTED1").GetSection("nested2")["keY"]);
        }
    }
}
