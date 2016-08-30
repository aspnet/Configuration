// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.Extensions.Configuration.AzureKeyVault
{
    /// <summary>
    /// 
    /// </summary>
    public class AzureKeyVaultConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Gets or sets the options required for retrieving values from the vault.
        /// </summary>
        public AzureKeyVaultOptions Options { get; set; }

        /// <inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            if (Options == null)
            {
                throw new ArgumentNullException(nameof(Options));
            }
            if (Options.ClientId == null)
            {
                throw new ArgumentNullException(nameof(Options.ClientId));
            }
            if (string.IsNullOrWhiteSpace(Options.VaultUri))
            {
                throw new ArgumentNullException(nameof(Options.VaultUri));
            }

            var keyVaultClient = CreateKeyVaultClient(Options);

            return new AzureKeyVaultConfigurationProvider(
                new KeyVaultClientWrapper(keyVaultClient), 
                Options.VaultUri,
                Options.VaultName,
                Options.IncludedSecrets,
                Options.ExcludedSecrets);
        }

        private KeyVaultClient CreateKeyVaultClient(AzureKeyVaultOptions options)
        {
            KeyVaultClient.AuthenticationCallback callback = null;

            if (!string.IsNullOrWhiteSpace(Options.ClientSecret))
            {
                callback = (authority, resource, scope) => GetTokenFromClientSecret(authority, resource, scope, Options.ClientId, Options.ClientSecret);
            }
            else if (Options.Certificate != null)
            {
                callback = (authority, resource, scope) => GetTokenFromClientCertificate(authority, resource, scope, Options.ClientId, Options.Certificate);
            }
            else if (!string.IsNullOrWhiteSpace(Options.Thumbprint))
            {
                // TODO validate other options
                // TODO load certificate based on options
                throw new NotImplementedException();
            }
            else
            {
                throw new ArgumentNullException(nameof(Options.ClientSecret));
            }

            return new KeyVaultClient(callback);
        }

        private static async Task<string> GetTokenFromClientSecret(string authority, string resource, string scope, string clientId, string clientSecret)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(clientId, clientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred);
            return result.AccessToken;
        }

        private static async Task<string> GetTokenFromClientCertificate(string authority, string resource, string scope, string clientId, X509Certificate2 certificate)
        {
            var authContext = new AuthenticationContext(authority);
            var result = await authContext.AcquireTokenAsync(resource, new ClientAssertionCertificate(clientId, certificate));
            return result.AccessToken;
        }
    }
}