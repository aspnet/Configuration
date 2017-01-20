// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Base helper class for implementing an <see cref="IConfigurationSource"/>
    /// </summary>
    public abstract class ConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Will be called if an uncaught exception occurs in Load.
        /// </summary>
        public Action<ConfigurationLoadExceptionContext> OnLoadException { get; set; }

        /// <summary>
        /// Builds the <see cref="IConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>An <see cref="IConfigurationProvider"/></returns>
        public abstract IConfigurationProvider Build(IConfigurationBuilder builder);
    }
}
