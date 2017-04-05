// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Fabric;

namespace Microsoft.Extensions.Configuration.ServiceFabric
{
    /// <summary>
    /// <see cref="ConfigurationProvider"/> for Service Fabric.
    /// </summary>
    public class ServiceFabricConfigurationProvider : ConfigurationProvider
    {
        private readonly ServiceContext _serviceContext;
        private readonly string _configurationPackageName;

        /// <summary>
        /// Initializes a new instance with Service Fabric context.
        /// </summary>
        /// <param name="serviceContext">The Service Fabric service context.</param>
        /// <param name="configurationPackageName">The name of the configuration package; defaults to "Config"
        /// if not provided.</param>
        public ServiceFabricConfigurationProvider(ServiceContext serviceContext,
            string configurationPackageName = null)
        {
            _serviceContext = serviceContext ?? throw new ArgumentNullException(nameof(serviceContext));
            _configurationPackageName = configurationPackageName ?? "Config";
        }

        /// <summary>
        /// Loads the Service Fabric settings.
        /// </summary>
        public override void Load()
        {
            var configurationPackage = _serviceContext.CodePackageActivationContext.GetConfigurationPackageObject(
                _configurationPackageName);

            foreach (var section in configurationPackage.Settings.Sections)
            {
                foreach (var parameter in section.Parameters)
                {
                    Data[$"{section.Name}{ConfigurationPath.KeyDelimiter}{parameter.Name}"] = parameter.Value;
                }
            }
        }
    }
}