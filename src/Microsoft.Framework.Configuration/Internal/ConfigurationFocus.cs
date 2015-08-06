// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Framework.Configuration.Internal
{
    public class ConfigurationFocus : IConfigurationSection
    {
        private string _key { get; set; }
        private string _value { get; set; }

        private readonly string _prefix;
        private readonly IConfigurationSection _root;

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
                    return Get(_key);
                }
            }

            set
            {
                Set(_key, value);
            }
        }

        public ConfigurationFocus(IConfigurationSection root, string prefix)
            : this(root, prefix, null)
        {
        }

        public ConfigurationFocus(IConfigurationSection root, string prefix, string key)
        {
            _prefix = prefix;
            _root = root;
            _key = key;
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

        public IConfigurationSection GetSection(string key)
        {
            return _root.GetSection(_prefix + key);
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            _root[_prefix.Substring(0, _prefix.Length - 1)] = string.Empty;
            return _root.GetChildren();
        }

        private string Get(string key)
        {
            // Null key indicates that the prefix passed to ctor should be used as a key
            if (key == null)
            {
                // Strip off the trailing colon to get a valid key
                var defaultKey = _prefix.Substring(0, _prefix.Length - 1);
                return _root[defaultKey];
            }

            return _root[_prefix + key];
        }

        private void Set(string key, string value)
        {
            _key = key;
            _value = value;
            _root[_prefix + key] = value;
        }
    }
}