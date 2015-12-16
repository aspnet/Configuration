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
            int pathStart;
            if (source is IConfigurationRoot)
            {
                pathStart = 0;
            }
            else
            {
                pathStart = source.Path.Length + 1;
            }
            foreach (var section in source.GetChildren())
            {
                AddSection(section, pathStart);
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
