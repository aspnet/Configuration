// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Rest.Azure;

namespace Microsoft.Extensions.Configuration.AzureKeyVault
{
    /// <inheritdoc />
    public class KeyVaultClientWrapper : IKeyVaultClient
    {
        private readonly Azure.KeyVault.IKeyVaultClient _keyVaultClientImplementation;

        /// <summary>
        /// Creates a new instance of <see cref="KeyVaultClientWrapper" />.
        /// </summary>
        /// <param name="keyVaultClientImplementation">The <see cref="IKeyVaultClient" /> instance to wrap.</param>
        public KeyVaultClientWrapper(Azure.KeyVault.IKeyVaultClient keyVaultClientImplementation)
        {
            _keyVaultClientImplementation = keyVaultClientImplementation;
        }

        /// <inheritdoc />
        public Task<IPage<SecretItem>> GetSecretsAsync(string vault)
        {
            return _keyVaultClientImplementation.GetSecretsAsync(vault);
        }

        /// <inheritdoc />
        public Task<SecretBundle> GetSecretAsync(string secretIdentifier)
        {
            return _keyVaultClientImplementation.GetSecretAsync(secretIdentifier);
        }

        /// <inheritdoc />
        public Task<IPage<SecretItem>> GetSecretsNextAsync(string nextLink)
        {
            return _keyVaultClientImplementation.GetSecretsNextAsync(nextLink);
        }
    }
}