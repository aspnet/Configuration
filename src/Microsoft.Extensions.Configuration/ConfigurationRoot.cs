// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration
{
    public class ConfigurationRoot : IConfigurationRoot
    {
        private IList<IConfigurationProvider> _providers;

        public IChangeMonitor<IConfigurationRoot> Monitor { get; }

        public ConfigurationRoot(IList<IConfigurationProvider> providers)
        {
            if (providers == null)
            {
                throw new ArgumentNullException(nameof(providers));
            }

            Monitor = new ChangeMonitor<IConfigurationRoot>(this);

            _providers = providers;
            foreach (var p in providers)
            {
                if (p.Monitor != null)
                {
                    // Raise the change event for the whole root when any provider changes
                    p.Monitor.RegisterOnChanged(_ => Monitor.RaiseChanged());
                }
                p.Load();
            }
        }

        public string this[string key]
        {
            get
            {
                foreach (var provider in _providers.Reverse())
                {
                    string value;

                    if (provider.TryGet(key, out value))
                    {
                        return value;
                    }
                }

                return null;
            }

            set
            {
                if (!_providers.Any())
                {
                    throw new InvalidOperationException(Resources.Error_NoSources);
                }

                foreach (var provider in _providers)
                {
                    provider.Set(key, value);
                }
            }
        }

        public IEnumerable<IConfigurationSection> GetChildren() => GetChildrenImplementation(null);

        internal IEnumerable<IConfigurationSection> GetChildrenImplementation(string path)
        {
            return _providers
                .Aggregate(Enumerable.Empty<string>(),
                    (seed, source) => source.GetChildKeys(seed, path))
                .Distinct()
                .Select(key => GetSection(path == null ? key : ConfigurationPath.Combine(path, key)));
        }

        public IConfigurationSection GetSection(string key)
        {
            return new ConfigurationSection(this, key);
        }

        public void Reload()
        {
            foreach (var provider in _providers)
            {
                provider.Load();
            }
            Monitor.RaiseChanged();
        }
    }
}
