// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Framework.Internal;

namespace Microsoft.Framework.Configuration
{
    public class ConfigurationRoot : ConfigurationBase, IConfigurationRoot
    {
        public ConfigurationRoot([NotNull] IList<IConfigurationSource> sources)
            : base(sources)
        {
        }

        public override IEnumerable<IConfigurationSection> GetChildren()
        {
            return new ConfigurationSection(Sources).GetChildren();
        }

        public override IConfigurationSection GetSection([NotNull] string key)
        {
            return new ConfigurationSection(Sources, key);
        }

        public void Reload()
        {
            foreach (var src in Sources)
            {
                src.Load();
            }
        }
    }
}
