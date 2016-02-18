// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;

namespace Microsoft.Extensions.Configuration.Test
{
    public class ConfigurationPathTest
    {
        [Fact]
        public void CombineWithEmptySegmentLeavesDelimiter()
        {
            Assert.Equal("parent:", ConfigurationPath.Combine("parent", ""));
            Assert.Equal("parent::", ConfigurationPath.Combine("parent", "", ""));
            Assert.Equal("parent:::key", ConfigurationPath.Combine("parent", "", "", "key"));
        }

        [Fact]
        public void GetLastSegmentTests()
        {
            Assert.Equal("", ConfigurationPath.GetLastSegment(":::"));
            Assert.Equal("c", ConfigurationPath.GetLastSegment("a::b:::c"));
            Assert.Equal("", ConfigurationPath.GetLastSegment("a:::b:"));
            Assert.Equal("key", ConfigurationPath.GetLastSegment("key"));
            Assert.Equal("key", ConfigurationPath.GetLastSegment(":key"));
            Assert.Equal("key", ConfigurationPath.GetLastSegment("::key"));
            Assert.Equal("key", ConfigurationPath.GetLastSegment("parent:key"));
        }

    }
}
