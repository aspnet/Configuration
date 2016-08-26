// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.KeyVault;

namespace Microsoft.Extensions.Configuration.AzureKeyVault
{
    /// <summary>
    /// 
    /// </summary>
    public class AzureKeyVaultConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Gets or sets the <see cref="KeyVaultClient"/> to use for retrieving values.
        /// </summary>
        public KeyVaultClient Client { get; set; }

        /// <summary>
        /// Gets or sets the vault uri.
        /// </summary>
        public string Vault { get; set; }

        /// <summary>
        /// Gets or sets The predicate to filter secret entries before loading value, <code>null</code> to load all.
        /// </summary>
        public Func<SecretItem, bool> Filter { get; set; }

        /// <inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureKeyVaultConfigurationProvider(new KeyVaultClientWrapper(Client), Vault, Filter);
        }
    }
}