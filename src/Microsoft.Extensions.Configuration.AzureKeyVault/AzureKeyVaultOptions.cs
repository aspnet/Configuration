// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.Configuration.AzureKeyVault
{
    /// <summary>
    /// Options for retrieving secrets from a Key Vault.
    /// </summary>
    public class AzureKeyVaultOptions
    {
        // TODO delimiter?

        /// <summary>
        /// The application client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Authentication via a client secret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Authentication via an already loaded client certificate.
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Authentication via a client certificate with the given <see cref="Thumbprint" /> that should be loaded
        /// from the given <see cref="StoreLocation" /> and <see cref="StoreName" />.
        /// </summary>
        public string Thumbprint { get; set; }

        /// <summary>
        /// Authentication via a client certificate with the given <see cref="Thumbprint" /> that should be loaded
        /// from the given <see cref="StoreLocation" /> and <see cref="StoreName" />.
        /// </summary>
        public StoreLocation? StoreLocation { get; set; }

        /// <summary>
        /// Authentication via a client certificate with the given <see cref="Thumbprint" /> that should be loaded
        /// from the given <see cref="StoreLocation" /> and <see cref="StoreName" />.
        /// </summary>
        public StoreName? StoreName { get; set; }

        /// <summary>
        /// Gets or sets the vault uri.
        /// </summary>
        public string VaultUri { get; set; }

        /// <summary>
        /// Gets or sets an optional internal name for the vault.
        /// Will be used as a path-prefix for all secrets.
        /// </summary>
        public string VaultName { get; set; }

        /// <summary>
        /// <para>Allows to only load the specified secrets.
        /// This also allows to write mappings between internal names and key vault names.</para>
        /// </summary>
        /// <remarks>
        /// Key: Internal name for the secret.
        /// Value: Name of the secret in Azure Key Vault. (leave empty if equal to internal name)
        /// </remarks>
        public Dictionary<string, string> IncludedSecrets { get; set; }

        /// <summary>
        /// Allows to load all secrets except the ones specified here.
        /// </summary>
        public List<string> ExcludedSecrets { get; set; }
    }
}