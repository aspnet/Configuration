// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Configuration
{
    public class IncludeConfigurationProvider : ConfigurationProvider
    {
        public IncludeConfigurationProvider(IConfiguration source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            foreach (var section in source.GetChildren())
            {
                AddSection(section, section.Key);
            }
        }

        private void AddSection(IConfigurationSection section, string keyPrefix)
        {
            Data.Add(keyPrefix, section.Value);
            foreach (var child in section.GetChildren())
            {
                AddSection(child, keyPrefix + Constants.KeyDelimiter + child.Key);
            }
        }
    }
}
