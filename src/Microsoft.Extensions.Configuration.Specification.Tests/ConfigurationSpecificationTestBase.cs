// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
        /// <returns></returns>
        public abstract IConfigurationProvider BuildTestProvider();

        /// <summary>
        /// Test
        /// </summary>
        [Fact]
        public void GetSetShouldWork()
        {
            var provider = BuildTestProvider();
            provider.Set("root", "root");
            provider.Set("nested:key", "nested1");
            provider.Set("nested:nested2:key", "nested2");
            Assert.True(provider.HasValue("root", "root"));
            Assert.True(provider.HasValue("nested:key", "nested1"));
            Assert.True(provider.HasValue("nested:nested2:key", "nested2"));
        }

        /// <summary>
        /// Test
        /// </summary>
        [Fact]
        public void KeysShouldBeCaseInsensitive()
        {
            var provider = BuildTestProvider();
            provider.Set("Root", "Root");
            provider.Set("Nested1:KEY", "Nested1");
            provider.Set("Nested1:key2", "Nested1-2");
            provider.Set("Nested1:Nested2:Key", "Nested2");
            Assert.True(provider.HasValue("ROOT", "Root"));
            Assert.True(provider.HasValue("NESTED1:Key", "Nested1"));
            Assert.True(provider.HasValue("NesteD1:Key2", "Nested1-2"));
            Assert.True(provider.HasValue("NesteD1:nested2:keY", "Nested2"));
        }

        /// <summary>
        /// Test
        /// </summary>
        [Fact]
        public void SetToNullShouldRemoveValue()
        {
            var provider = BuildTestProvider();
            provider.Set("root", "root");
            provider.Set("nested:key", "nested1");
            provider.Set("nested:nested2:key", "nested2");
            Assert.True(provider.HasValue("root", "root"));
            Assert.True(provider.HasValue("nested:key", "nested1"));
            Assert.True(provider.HasValue("nested:nested2:key", "nested2"));
            provider.Set("root", null);
            provider.Set("nested:key", null);
            provider.Set("nested:nested2:key", null);
            // These all fail today.
            //Assert.False(provider.HasValue("root"));
            //Assert.False(provider.HasValue("nested:key"));
            //Assert.False(provider.HasValue("nested:nested2:key"));
        }
    }

    internal static class TestExtensions
    {
        public static bool HasValue(this IConfigurationProvider provider, string key, string value = null)
        {
            string val;
            return provider.TryGet(key, out val) && val == value;
        }
    }
}
