// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Framework.Configuration
{
    public class ConfigurationSection : ConfigurationBase, IConfigurationSection
    {
        private readonly string _key;
        private readonly string _parentPath;

        public ConfigurationSection(IList<IConfigurationSource> sources, string parentPath, string key)
            : base(sources)
        {
            if (sources == null)
            {
                throw new ArgumentNullException(nameof(sources));
            }

            if (parentPath == null)
            {
                throw new ArgumentNullException(nameof(parentPath));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Resources.Error_EmptyKey);
            }

            _key = key;
            _parentPath = parentPath;
        }

        public string Key
        {
            get
            {
                return _key;
            }
        }

        public override string Path
        {
            get
            {
                if (!string.IsNullOrEmpty(_parentPath))
                {
                    return _parentPath + _key;
                }
                else
                {
                    return _key;
                }
            }
        }

        public string Value
        {
            get
            {
                foreach (var src in Sources.Reverse())
                {
                    string value = null;

                    if (src.TryGet(Path, out value))
                    {
                        return value;
                    }
                }

                return null;
            }
            set
            {
                if (!Sources.Any())
                {
                    throw new InvalidOperationException(Resources.Error_NoSources);
                }

                foreach (var src in Sources)
                {
                    src.Set(Path, value);
                }
            }
        }
    }
}
