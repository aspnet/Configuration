// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Configuration
{
    public class IncludedConfigurationProvider : ConfigurationProvider
    {
        public IncludedConfigurationProvider(IConfiguration source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            int pathStart = 0;
            var section = source as IConfigurationSection;
            if (section != null)
            {
                pathStart = section.Path.Length + 1;
            }
            foreach (var child in source.GetChildren())
            {
                AddSection(child, pathStart);
            }
        }

        private void AddSection(IConfigurationSection section, int pathStart)
        {
            Data.Add(section.Path.Substring(pathStart), section.Value);
            foreach (var child in section.GetChildren())
            {
                AddSection(child, pathStart);
            }
        }
    }
}
