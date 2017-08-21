﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Configuration.AzureKeyVault
{
    /// <summary>
    /// Represents Azure KeyVault secrets as an <see cref="IConfigurationSource" />.
    /// </summary>
    internal class AzureKeyVaultConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Gets or sets the <see cref="IKeyVaultClient" /> to use for retrieving values.
        /// </summary>
        public IKeyVaultClient Client { get; set; }

        /// <summary>
        /// Gets or sets the vault uri.
        /// </summary>
        public string Vault { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IKeyVaultSecretManager" /> instance used to control secret loading.
        /// </summary>
        public IKeyVaultSecretManager Manager { get; set; }

        /// <inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureKeyVaultConfigurationProvider(Client, Vault, Manager);
        }
    }
}