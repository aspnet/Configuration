// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Fabric;

namespace Microsoft.Extensions.Configuration.ServiceFabric
{
    /// <summary>
    /// Represents Service Fabric settings as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class ServiceFabricConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// The Service Fabric service context.
        /// </summary>
        public ServiceContext ServiceContext { get; set; }

        /// <summary>
        /// Builds the <see cref="ServiceFabricConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="ServiceFabricConfigurationProvider"/></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ServiceFabricConfigurationProvider(ServiceContext);
        }
    }
}