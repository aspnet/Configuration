// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Used to build key/value based configuration settings for use in an application.
    /// </summary>
    public class ConfigurationBuilder : IConfigurationBuilder
    {
        private readonly IList<IConfigurationSource> _sources = new List<IConfigurationSource>();

        /// <summary>
        /// Gets or sets the source at the specified index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The source at the specified index.</returns>
        public IConfigurationSource this[int index] { get => _sources[index]; set => _sources[index] = value; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between the <see cref="IConfigurationBuilder"/>
        /// and the registered <see cref="IConfigurationProvider"/>s.
        /// </summary>
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        /// <summary>
        /// The number of sources in this builder.
        /// </summary>
        public int Count => _sources.Count;

        /// <summary>
        /// This list is not read only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Returns the sources used to obtain configuration values.
        /// </summary>
        public IEnumerable<IConfigurationSource> Sources => _sources;

        /// <summary>
        /// Adds a new configuration source.
        /// </summary>
        /// <param name="source">The configuration source to add.</param>
        /// <returns>The same <see cref="IConfigurationBuilder"/>.</returns>
        public IConfigurationBuilder Add(IConfigurationSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            _sources.Add(source);
            return this;
        }

        /// <summary>
        /// Builds an <see cref="IConfiguration"/> with keys and values from the set of providers registered in
        /// this builder.
        /// </summary>
        /// <returns>An <see cref="IConfigurationRoot"/> with keys and values from the registered providers.</returns>
        public IConfigurationRoot Build()
        {
            var providers = new List<IConfigurationProvider>();
            foreach (var source in _sources)
            {
                var provider = source.Build(this);
                providers.Add(provider);
            }
            return new ConfigurationRoot(providers);
        }

        /// <summary>
        /// Removes all the sources in the builder.
        /// </summary>
        public void Clear() => _sources.Clear();

        /// <summary>
        /// Returns true if the builder contains this source.
        /// </summary>
        /// <param name="item">The source.</param>
        /// <returns></returns>
        public bool Contains(IConfigurationSource item) => _sources.Contains(item);

        /// <summary>
        /// Copies the provided sources to the specified index.
        /// </summary>
        /// <param name="array">The sources.</param>
        /// <param name="arrayIndex">The index.</param>
        public void CopyTo(IConfigurationSource[] array, int arrayIndex) => _sources.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator of sources.
        /// </summary>
        /// <returns>An enumerator of sources.</returns>
        public IEnumerator<IConfigurationSource> GetEnumerator() => _sources.GetEnumerator();

        /// <summary>
        /// Returns the index of the source, or -1 if not found.
        /// </summary>
        /// <param name="item">The source.</param>
        /// <returns>The index of the source, or -1 if not found</returns>
        public int IndexOf(IConfigurationSource item) => _sources.IndexOf(item);

        /// <summary>
        /// Inserts a source at a specified index.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="item">The source to insert.</param>
        public void Insert(int index, IConfigurationSource item) => _sources.Insert(index, item);

        /// <summary>
        /// Remove a source.
        /// </summary>
        /// <param name="item">The source to remove.</param>
        /// <returns>Whether the source was removed.</returns>
        public bool Remove(IConfigurationSource item) => _sources.Remove(item);

        /// <summary>
        /// Remove the source at the specified index.
        /// </summary>
        /// <param name="index">The index of the source to remove.</param>
        public void RemoveAt(int index) => _sources.RemoveAt(index);

        /// <summary>
        /// Add a source to the builder.
        /// </summary>
        /// <param name="item">The source to add.</param>
        void ICollection<IConfigurationSource>.Add(IConfigurationSource item)
            => Add(item);

        IEnumerator IEnumerable.GetEnumerator() => _sources.GetEnumerator();
    }
}
