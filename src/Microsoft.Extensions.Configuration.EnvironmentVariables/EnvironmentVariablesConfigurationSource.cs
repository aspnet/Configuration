// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Configuration.EnvironmentVariables
{
    /// <summary>
    /// A environment variable based <see cref="IConfigurationSource"/>.
    /// </summary>
    public class EnvironmentVariablesConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// The prefix to append to the keys.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Builds the <see cref="EnvironmentVariablesConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="EnvironmentVariablesConfigurationProvider"/></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EnvironmentVariablesConfigurationProvider(Prefix);
        }
    }
}
