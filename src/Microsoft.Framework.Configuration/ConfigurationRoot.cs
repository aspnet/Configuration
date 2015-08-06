// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Internal;

namespace Microsoft.Framework.Configuration
{
    public class ConfigurationRoot : IConfigurationRoot
    {
        private readonly IList<IConfigurationSource> _sources = new List<IConfigurationSource>();

        public ConfigurationRoot(IList<IConfigurationSource> sources)
        {
            _sources = sources;
        }

        public IEnumerable<IConfigurationSource> Sources
        {
            get
            {
                return _sources;
            }
        }

        public string this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return new ConfigurationSection(_sources).GetSection(string.Empty).GetChildren();
        }

        public IConfigurationSection GetSection(string key)
        {
            return new ConfigurationSection(_sources).GetSection(key);
        }

        public void Reload()
        {
            foreach (var src in _sources)
            {
                src.Load();
            }
        }

        private string Get([NotNull] string key)
        {
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

        private void Set([NotNull] string key, [NotNull] string value)
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
}
