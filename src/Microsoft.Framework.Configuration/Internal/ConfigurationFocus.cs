// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Framework.Configuration.Internal
{
    public class ConfigurationFocus : IConfiguration
    {
        private readonly string _prefix;
        private readonly IConfiguration _root;

        public ConfigurationFocus(IConfiguration root, string prefix)
        {
            _prefix = prefix;
            _root = root;
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

        public string Get(string key)
        {
            // Null key indicates that the prefix passed to ctor should be used as a key
            if (key == null)
            {
                // Strip off the trailing colon to get a valid key
                var defaultKey = _prefix.Substring(0, _prefix.Length - 1);
                return _root.Get(defaultKey);
            }

            return _root.Get(_prefix + key);
        }

        public bool TryGet(string key, out string value)
        {
            // Null key indicates that the prefix passed to ctor should be used as a key
            if (key == null)
            {
                // Strip off the trailing colon to get a valid key
                var defaultKey = _prefix.Substring(0, _prefix.Length - 1);
                return _root.TryGet(defaultKey, out value);
            }
            return _root.TryGet(_prefix + key, out value);
        }

        public IConfiguration GetConfigurationSection(string key)
        {
            return _root.GetConfigurationSection(_prefix + key);
        }

        public void Set(string key, string value)
        {
            _root.Set(_prefix + key, value);
        }

        public IEnumerable<KeyValuePair<string, IConfiguration>> GetConfigurationSections()
        {
            return _root.GetConfigurationSections(_prefix.Substring(0, _prefix.Length - 1));
        }

        public IEnumerable<KeyValuePair<string, IConfiguration>> GetConfigurationSections(string key)
        {
            return _root.GetConfigurationSections(_prefix + key);
        }

        public void Reload()
        {
            throw new NotImplementedException();
        }
    }
}