// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Represents a list of <see cref="IConfigurationSource"/>s used to build application configuration.
    /// </summary>
    public interface IConfigurationBuilder : IList<IConfigurationSource>
    {
        /// <summary>
        /// Gets a key/value collection that can be used to share data between the <see cref="IConfigurationBuilder"/>
        /// and the registered <see cref="IConfigurationSource"/>s.
        /// </summary>
        Dictionary<string, object> Properties { get; }

        /// <summary>
        /// Gets the sources used to obtain configuration values
        /// </summary>
        IEnumerable<IConfigurationSource> Sources { get; }

        /// <summary>
        /// Adds a new configuration source.
        /// </summary>
        /// <param name="source">The configuration source to add.</param>
        /// <returns>The same <see cref="IConfigurationBuilder"/>.</returns>
        new IConfigurationBuilder Add(IConfigurationSource source);

        /// <summary>
        /// Builds an <see cref="IConfiguration"/> with keys and values from the set of sources registered in
        /// this builder.
        /// </summary>
        /// <returns>An <see cref="IConfigurationRoot"/> with keys and values from the registered sources.</returns>
        IConfigurationRoot Build();
    }
}
