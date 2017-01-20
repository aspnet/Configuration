// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;

namespace Microsoft.Extensions.Configuration.AzureKeyVault
{
    /// <summary>
    /// An AzureKeyVault based <see cref="IConfigurationProvider"/>.
    /// </summary>
    internal class AzureKeyVaultConfigurationProvider : ConfigurationProvider<AzureKeyVaultConfigurationSource>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AzureKeyVaultConfigurationProvider"/>.
        /// </summary>
        /// <param name="client">The <see cref="KeyVaultClient"/> to use for retrieving values.</param>
        /// <param name="vault">Azure KeyVault uri.</param>
        /// <param name="manager"></param>
        public AzureKeyVaultConfigurationProvider(IKeyVaultClient client, string vault, IKeyVaultSecretManager manager)
            : this(new AzureKeyVaultConfigurationSource { Client = client, Vault = vault, Manager = manager })
        { }

        public AzureKeyVaultConfigurationProvider(AzureKeyVaultConfigurationSource source) : base(source)
        {
            if (source.Client == null)
            {
                throw new ArgumentNullException(nameof(source.Client));
            }
            if (source.Vault == null)
            {
                throw new ArgumentNullException(nameof(source.Vault));
            }
        }

        /// <inheritdoc />
        public override void Load()
        {
            LoadAsync().GetAwaiter().GetResult();
        }

        private IKeyVaultClient Client => Source.Client;
        private string Vault => Source.Vault;
        private IKeyVaultSecretManager Manager => Source.Manager;

        private async Task LoadAsync()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var secrets = await Client.GetSecretsAsync(Vault);
            do
            {
                foreach (var secretItem in secrets)
                {
                    if (!Manager.Load(secretItem))
                    {
                        continue;
                    }

                    var value = await Client.GetSecretAsync(secretItem.Id);
                    var key = Manager.GetKey(value);
                    data.Add(key, value.Value);
                }

                secrets = secrets.NextPageLink != null ?
                    await Client.GetSecretsNextAsync(secrets.NextPageLink) :
                    null;
            } while (secrets != null);

            Data = data;
        }
    }
}