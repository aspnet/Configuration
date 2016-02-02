// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.Configuration
{
    public abstract class ConfigurationProvider : IConfigurationProvider
    {
        protected ConfigurationProvider()
        {
            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        protected IDictionary<string, string> Data { get; set; }

        public virtual bool TryGet(string key, out string value)
        {
            return Data.TryGetValue(key, out value);
        }

        public virtual void Set(string key, string value)
        {
            Data[key] = value;
        }

        public virtual void Load()
        {
        }

        public virtual void Initialize(IConfigurationRoot root)
        {
        }
       
        public virtual IEnumerable<string> GetChildKeys(
            IEnumerable<string> earlierKeys,
            string parentPath,
            string delimiter)
        {
            var prefix = parentPath == null ? string.Empty : parentPath + delimiter;

            return Data
                .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(kv => Segment(kv.Key, prefix, delimiter))
                .Concat(earlierKeys)
                .OrderBy(k => k, ConfigurationKeyComparer.Instance);
        }

        private static string Segment(string key, string prefix, string delimiter)
        {
            var indexOf = key.IndexOf(delimiter, prefix.Length, StringComparison.OrdinalIgnoreCase);
            return indexOf < 0 ? key.Substring(prefix.Length) : key.Substring(prefix.Length, indexOf - prefix.Length);
        }
    }
}
