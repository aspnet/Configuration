using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration.Test
{
    internal class FakeProvider : IConfigurationProvider
    {
        public bool Loaded { get; private set; }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath, string delimiter)
        {
            throw new NotImplementedException();
        }

        public void Load() => Loaded = true;

        public void Set(string key, string value)
        {
            throw new NotImplementedException();
        }

        public bool TryGet(string key, out string value)
        {
            throw new NotImplementedException();
        }
    }
}
