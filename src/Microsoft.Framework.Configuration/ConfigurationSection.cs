// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Internal;

namespace Microsoft.Framework.Configuration
{
    public class ConfigurationSection : ConfigurationBase, IConfigurationSection
    {
        private string _key;

        public ConfigurationSection([NotNull] IList<IConfigurationSource> sources)
            : base(sources)
        {
            _key = string.Empty;
            Value = null;
        }
        public ConfigurationSection([NotNull] IList<IConfigurationSource> sources, [NotNull] string key)
            : base (sources)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException(Resources.Error_EmptyKey);
            }

            _key = key;
            Value = base[_key];
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
            get; set;
        }

        public override string this[[NotNull] string key]
        {
            get
            {
                var prefix = _key;

                if (!string.IsNullOrEmpty(prefix))
                {
                    prefix = prefix + Constants.KeyDelimiter;
                }
                else
                {
                    prefix = string.Empty;
                }

                return base[prefix + key];
            }
            set
            {
                if (!Sources.Any())
                {
                    throw new InvalidOperationException(Resources.Error_NoSources);
                }

                var prefix = _key;

                if (!string.IsNullOrEmpty(prefix))
                {
                    prefix = prefix + Constants.KeyDelimiter;
                }
                else
                {
                    prefix = string.Empty;
                }

                _key = prefix + key;

                foreach (var src in Sources)
                {
                    src.Set(key, value);
                }
            }
        }

        public override IConfigurationSection GetSection([NotNull] string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException(Resources.Error_EmptyKey);
            }

            var prefix = Key;
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix = prefix + Constants.KeyDelimiter;
            }
            else
            {
                prefix = string.Empty;
            }
            return new ConfigurationSection(Sources, prefix + key);
        }

        public override IEnumerable<IConfigurationSection> GetChildren()
        {
            var prefix =Key + Constants.KeyDelimiter;

            var segments = Sources.Aggregate(
                Enumerable.Empty<string>(),
                (seed, source) => source.ProduceConfigurationSections(seed, prefix, Constants.KeyDelimiter));

            var distinctSegments = segments.Distinct();

            return distinctSegments.Select(segment => CreateConfigurationSection(prefix, segment));
        }

        private IConfigurationSection CreateConfigurationSection(string prefix, string segment)
        {
            return new ConfigurationSection(Sources, prefix + segment);
        }
    }
}
