// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Fabric;

namespace Microsoft.Extensions.Configuration.ServiceFabric
{
    /// <summary>
    /// Extension methods for registering <see cref="ServiceFabricConfigurationProvider"/> with
    /// <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class ServiceFabricExtensions
    {
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Service Fabric settings.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="serviceContext">The Service Fabric service context</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddServiceFabric(this IConfigurationBuilder configurationBuilder,
            ServiceContext serviceContext)
        {
            configurationBuilder.Add(new ServiceFabricConfigurationSource { ServiceContext = serviceContext });
            return configurationBuilder;
        }
    }
}