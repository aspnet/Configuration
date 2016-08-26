// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;

namespace Microsoft.Extensions.Configuration.AzureKeyVault
{
    /// <summary>
    /// An AzureKeyVault based <see cref="ConfigurationProvider"/>.
    /// </summary>
    public class AzureKeyVaultConfigurationProvider : ConfigurationProvider
    {
        private readonly IKeyVaultClient _client;
        private readonly string _vault;
        private readonly Func<SecretItem, bool> _filter;

        /// <summary>
        /// Creates a new instance of <see cref="AzureKeyVaultConfigurationProvider"/>.
        /// </summary>
        /// <param name="client">The <see cref="KeyVaultClient"/> to use for retrieving values.</param>
        /// <param name="vault">Azure KeyVault uri.</param>
        /// <param name="filter">The predicate to filter secret entries before loading value, <code>null</code> to load all.</param>
        public AzureKeyVaultConfigurationProvider(IKeyVaultClient client, string vault, Func<SecretItem, bool> filter)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (vault == null)
            {
                throw new ArgumentNullException(nameof(vault));
            }
            _client = client;
            _vault = vault;
            _filter = filter;
        }

        /// <inheritdoc />
        public override void Load()
        {
            LoadAsync().GetAwaiter().GetResult();
        }

        private async Task LoadAsync()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var secrets = await _client.GetSecretsAsync(_vault);
            do
            {
                foreach (var secretItem in secrets.Value)
                {
                    if (_filter?.Invoke(secretItem) == false)
                    {
                        continue;
                    }

                    var value = await _client.GetSecretAsync(secretItem.Id);
                    data.Add(value.SecretIdentifier.Name, value.Value);
                }

                secrets = secrets.NextLink != null ?
                    await _client.GetSecretsNextAsync(secrets.NextLink) :
                    null;
            } while (secrets != null);

            Data = data;
        }
    }
}