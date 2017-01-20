// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Base helper class for implementing an <see cref="IConfigurationProvider"/>
    /// </summary>
    /// <typeparam name="TSource">The type of <see cref="IConfigurationSource"/> used to build this provider.</typeparam>
    public abstract class ConfigurationProvider<TSource> : IConfigurationProvider where TSource : IConfigurationSource
    {
        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();

        /// <summary>
        /// Initializes a new <see cref="IConfigurationProvider"/>
        /// </summary>
        protected ConfigurationProvider(TSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Source = source;
        }

        /// <summary>
        /// The <see cref="IConfigurationSource"/> used to build this provider.
        /// </summary>
        public TSource Source { get; }

        /// <summary>
        /// The configuration key value pairs for this provider.
        /// </summary>
        protected IDictionary<string, string> Data { get; set; }

        /// <summary>
        /// Will be called if an uncaught exception occurs in Load.
        /// </summary>
        protected Action<ConfigurationLoadExceptionContext> OnLoadException { get; set; }

        /// <summary>
        /// Attempts to find a value with the given key, returns true if one is found, false otherwise.
        /// </summary>
        /// <param name="key">The key to lookup.</param>
        /// <param name="value">The value found at key if one is found.</param>
        /// <returns>True if key has a value, false otherwise.</returns>
        public virtual bool TryGet(string key, out string value)
        {
            return Data.TryGetValue(key, out value);
        }

        /// <summary>
        /// Sets a value for a given key.
        /// </summary>
        /// <param name="key">The configuration key to set.</param>
        /// <param name="value">The value to set.</param>
        public virtual void Set(string key, string value)
        {
            Data[key] = value;
        }

        /// <summary>
        /// Loads (or reloads) the data for this provider.
        /// </summary>
        public virtual void Load()
        {
        }

        /// <summary>
        /// Returns the list of keys that this provider has.
        /// </summary>
        /// <param name="earlierKeys">The earlier keys that other providers contain.</param>
        /// <param name="parentPath">The path for the parent IConfiguration.</param>
        /// <returns>The list of keys for this provider.</returns>
        public virtual IEnumerable<string> GetChildKeys(
            IEnumerable<string> earlierKeys,
            string parentPath)
        {
            var prefix = parentPath == null ? string.Empty : parentPath + ConfigurationPath.KeyDelimiter;

            return Data
                .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(kv => Segment(kv.Key, prefix.Length))
                .Concat(earlierKeys)
                .OrderBy(k => k, ConfigurationKeyComparer.Instance);
        }

        private static string Segment(string key, int prefixLength)
        {
            var indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
            return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
        }

        /// <summary>
        /// Returns a <see cref="IChangeToken"/> that can be used to listen when this provider is reloaded.
        /// </summary>
        /// <returns></returns>
        public IChangeToken GetReloadToken()
        {
            return _reloadToken;
        }

        /// <summary>
        /// Triggers the reload change token and creates a new one.
        /// </summary>
        protected void OnReload()
        {
            var previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
            previousToken.OnReload();
        }

        IConfigurationSource IConfigurationProvider.Source
        {
            get
            {
                return Source;
            }
        }

    }
}
