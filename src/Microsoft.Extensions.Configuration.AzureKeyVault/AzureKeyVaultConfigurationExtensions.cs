// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for registering <see cref="AzureKeyVaultConfigurationProvider"/> with <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class AzureKeyVaultConfigurationExtensions
    {
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from the Azure KeyVault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Azure KeyVault uri.</param>
        /// <param name="clientId">The application client id.</param>
        /// <param name="clientSecret">The client secret to use for authentication.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAzureKeyVault(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string clientId,
            string clientSecret)
        {
            if (vaultUri == null)
            {
                throw new ArgumentNullException(nameof(vaultUri));
            }
            if (clientId == null)
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            if (clientSecret == null)
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

            var options = new AzureKeyVaultOptions
            {
                VaultUri = vaultUri,
                ClientId = clientId,
                ClientSecret = clientSecret
            };

            return configurationBuilder.AddAzureKeyVault(options);
        }


        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from the Azure KeyVault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">Azure KeyVault uri.</param>
        /// <param name="clientId">The application client id.</param>
        /// <param name="certificate">The <see cref="X509Certificate2"/> to use for authentication.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAzureKeyVault(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string clientId,
            X509Certificate2 certificate)
        {
            if (vaultUri == null)
            {
                throw new ArgumentNullException(nameof(vaultUri));
            }
            if (clientId == null)
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            var options = new AzureKeyVaultOptions
            {
                VaultUri = vaultUri,
                ClientId = clientId,
                Certificate = certificate
            };

            return configurationBuilder.AddAzureKeyVault(options);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from the Azure KeyVault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="options">The options for retrieving values from the vault.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAzureKeyVault(
            this IConfigurationBuilder configurationBuilder,
            IConfiguration options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var keyVaultOptions = new AzureKeyVaultOptions();
            options.Bind(keyVaultOptions);

            return configurationBuilder.AddAzureKeyVault(keyVaultOptions);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from the Azure KeyVault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="options">The options for retrieving values from the vault.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAzureKeyVault(
            this IConfigurationBuilder configurationBuilder,
            AzureKeyVaultOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            configurationBuilder.Add(new AzureKeyVaultConfigurationSource()
            {
                Options = options
            });

            return configurationBuilder;
        }
    }
}