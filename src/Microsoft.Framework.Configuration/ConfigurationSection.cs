// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Configuration.Internal;
using Microsoft.Framework.Internal;

namespace Microsoft.Framework.Configuration
{
    public class ConfigurationSection : IConfigurationSection
    {
        private readonly IList<IConfigurationSource> _sources = new List<IConfigurationSource>();
        private string _key { get; set; }
        private string _value { get; set; }

        public ConfigurationSection(IList<IConfigurationSource> sources)
        {
            _sources = sources;
        }

        public string this[string key]
        {
            get
            {
                _value = Get(key);
                return _value;
            }
            set
            {
                Set(key, value);
            }
        }

        public IEnumerable<IConfigurationSource> Sources
        {
            get
            {
                return _sources;
            }
        }

        public string Key
        {
            get
            {
                return _key;
            }
        }

        public string Value
        {
            get
            {
                if (_value != null)
                {
                    return _value;
                }
                else
                {
                    _value = Get(_key);
                    return _value;
                }
            }

            set
            {
                Set(_key, value);
            }
        }

        public IConfigurationSection GetSection(string key)
        {
            return new ConfigurationFocus(this, key + Constants.KeyDelimiter);
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return GetChildrenImplementation(_key + Constants.KeyDelimiter);
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

            _key = key;
            _value = value;

            foreach (var src in _sources)
            {
                src.Set(key, value);
            }
        }

        private IEnumerable<IConfigurationSection> GetChildrenImplementation(string prefix)
        {
            var segments = _sources.Aggregate(
                Enumerable.Empty<string>(),
                (seed, source) => source.ProduceConfigurationSections(seed, prefix, Constants.KeyDelimiter));

            var distinctSegments = segments.Distinct();

            return distinctSegments.Select(segment => CreateConfigurationFocus(prefix, segment));
        }

        private IConfigurationSection CreateConfigurationFocus(string prefix, string segment)
        {
            return new ConfigurationFocus(this, prefix + segment + Constants.KeyDelimiter, segment);
        }
    }
}
