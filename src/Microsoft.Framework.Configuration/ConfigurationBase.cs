// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Internal;

namespace Microsoft.Framework.Configuration
{
    public abstract class ConfigurationBase : IConfiguration
    {
        private readonly IList<IConfigurationSource> _sources = new List<IConfigurationSource>();

        public ConfigurationBase ([NotNull] IList<IConfigurationSource> sources)
        {
            _sources = sources;
        }

        public virtual string this[[NotNull] string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new InvalidOperationException(Resources.Error_EmptyKey);
                }

                // If a key in the newly added configuration source is identical to a key in a
                // formerly added configuration source, the new one overrides the former one.
                // So we search in reverse order, starting with latest configuration source.
                foreach (var src in _sources.Reverse())
                {
                    string value = null;

                    if (src.TryGet(key, out value))
                    {
                        return value;
                    }
                }

                return null;
            }
            set
            {
                if (!_sources.Any())
                {
                    throw new InvalidOperationException(Resources.Error_NoSources);
                }

                foreach (var src in _sources)
                {
                    src.Set(key, value);
                }
            }
        }

        public IList<IConfigurationSource> Sources
        {
            get
            {
                return _sources;
            }
        }

        public abstract IEnumerable<IConfigurationSection> GetChildren();

        public abstract IConfigurationSection GetSection(string key);
    }
}
