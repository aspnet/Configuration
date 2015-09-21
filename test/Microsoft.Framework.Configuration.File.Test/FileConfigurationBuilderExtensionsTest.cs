// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;

namespace Microsoft.Framework.Configuration.Json
{
    public class FileConfigurationBuilderExtensionsTest
    {
        [Fact]
        public void SetBasePath_ThrowsIfBasePathIsNull()
        {
            // Arrange
            var builder = new ConfigurationBuilder();

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => builder.SetBasePath(null));
            Assert.Equal("basePath", ex.ParamName);
        }
    }
}
