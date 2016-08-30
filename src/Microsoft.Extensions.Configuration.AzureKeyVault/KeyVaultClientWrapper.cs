﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;

namespace Microsoft.Extensions.Configuration.AzureKeyVault
{
    /// <inheritdoc />
    public class KeyVaultClientWrapper : IKeyVaultClient
    {
        private readonly KeyVaultClient _keyVaultClientImplementation;

        /// <summary>
        /// Creates a new instance of <see cref="KeyVaultClientWrapper"/>.
        /// </summary>
        /// <param name="keyVaultClientImplementation">The <see cref="KeyVaultClient"/> instance to wrap.</param>
        public KeyVaultClientWrapper(KeyVaultClient keyVaultClientImplementation)
        {
            _keyVaultClientImplementation = keyVaultClientImplementation;
        }

        /// <inheritdoc />
        public Task<ListSecretsResponseMessage> GetSecretsAsync(string vault)
        {
            return _keyVaultClientImplementation.GetSecretsAsync(vault);
        }

        /// <inheritdoc />
        public Task<Secret> GetSecretAsync(string secretIdentifier)
        {
            return _keyVaultClientImplementation.GetSecretAsync(secretIdentifier);
        }

        /// <inheritdoc />
        public Task<Secret> GetSecretAsync(string vaultUri, string secretName)
        {
            return _keyVaultClientImplementation.GetSecretAsync(vaultUri, secretName);
        }

        /// <inheritdoc />
        public Task<ListSecretsResponseMessage> GetSecretsNextAsync(string nextLink)
        {
            return _keyVaultClientImplementation.GetSecretsNextAsync(nextLink);
        }
    }
}