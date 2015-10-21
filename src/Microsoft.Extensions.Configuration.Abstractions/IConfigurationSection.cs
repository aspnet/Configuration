// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// REVIEW: Really not confident these are correct.
    /// Represents a sub-section of application configuration values.
    /// </summary>
    public interface IConfigurationSection : IConfiguration
    {
        /// <summary>
        /// Gets the key this sub-section was retrieved from.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the path to the sub-section within the <see cref="IConfiguration"/> it was retrieved from.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        string Value { get; set; }
    }
}
