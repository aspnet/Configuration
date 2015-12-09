// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Configuration
{
    public class ChainedConfigurationProvider : ConfigurationProvider
    {
        private readonly IConfiguration _config;

        public ChainedConfigurationProvider(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            _config = config;
        }

        public override void Load()
        {
            foreach (var section in _config.GetChildren())
            {
                AddSection(section, section.Key);
            }
        }

        private void AddSection(IConfigurationSection section, string prefix)
        {
            Data.Add(prefix, section.Value);
            foreach (var child in section.GetChildren())
            {
                AddSection(child, prefix + Constants.KeyDelimiter + child.Key);
            }
        }
    }
}
