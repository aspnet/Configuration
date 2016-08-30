﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;

namespace Microsoft.Extensions.Configuration.AzureKeyVault
{
    /// <summary>
    /// Client class to perform cryptographic key operations and vault operations
    /// against the Key Vault service.
    /// Thread safety: This class is thread-safe.
    /// </summary>
    public interface IKeyVaultClient
    {
        /// <summary>List secrets in the specified vault</summary>
        /// <param name="vaultUri">The URL for the vault containing the secrets.</param>
        /// <returns>A response message containing a list of secrets in the vault along with a link to the next page of secrets</returns>
        Task<ListSecretsResponseMessage> GetSecretsAsync(string vaultUri);

        /// <summary>Gets a secret by identifier.</summary>
        /// <param name="secretIdentifier">The URL for the secret.</param>
        /// <returns>A response message containing the secret</returns>
        Task<Secret> GetSecretAsync(string secretIdentifier);

        /// <summary>Gets a secret by name.</summary>
        /// <param name="vaultUri">The URL for the vault containing the secrets.</param>
        /// <param name="secretName">The name for the secret.</param>
        /// <returns>A response message containing the secret</returns>
        Task<Secret> GetSecretAsync(string vaultUri, string secretName);

        /// <summary>List the next page of secrets</summary>
        /// <param name="nextLink">nextLink value from a previous call to GetSecrets or GetSecretsNext</param>
        /// <returns>A response message containing a list of secrets in the vault along with a link to the next page of secrets</returns>
        Task<ListSecretsResponseMessage> GetSecretsNextAsync(string nextLink);
    }
}