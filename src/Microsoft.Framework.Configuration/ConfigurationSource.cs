// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Framework.Configuration
{
    public abstract class ConfigurationSource : IConfigurationSource
    {
        protected ConfigurationSource()
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
       
        public virtual IEnumerable<string> ProduceConfigurationSections(
            IEnumerable<string> earlierKeys,
            string parentPath,
            string delimiter)
        {
            parentPath = parentPath + delimiter;

            return Data
                .Where(kv => kv.Key.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase))
                .Select(kv => Segment(kv.Key, parentPath, delimiter))
                .Concat(earlierKeys)
                .OrderBy(k => k, ConfigurationKeyComparer.Instance);
        }

        private static string Segment(string key, string parentPath, string delimiter)
        {
            var indexOf = key.IndexOf(delimiter, parentPath.Length, StringComparison.OrdinalIgnoreCase);
            return indexOf < 0 ? key.Substring(parentPath.Length) : key.Substring(parentPath.Length, indexOf - parentPath.Length);
        }
    }
}
