// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration.Memory
{
    public class MemoryConfigurationSource : IConfigurationSource
    {
        public IEnumerable<KeyValuePair<string, string>> InitialData { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MemoryConfigurationProvider(InitialData);
        }

        private class MemoryConfigurationProvider :
            ConfigurationProvider,
            IEnumerable<KeyValuePair<string, string>>
        {
            public MemoryConfigurationProvider(IEnumerable<KeyValuePair<string, string>> initialData)
            {
                if (initialData != null)
                {
                    foreach (var pair in initialData)
                    {
                        Data.Add(pair.Key, pair.Value);
                    }
                }
            }

            public void Add(string key, string value)
            {
                Data.Add(key, value);
            }

            public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            {
                return Data.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

    }
}
