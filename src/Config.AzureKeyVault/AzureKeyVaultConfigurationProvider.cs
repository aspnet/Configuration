// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;

namespace Microsoft.Extensions.Configuration.AzureKeyVault
{
    /// <summary>
    /// An AzureKeyVault based <see cref="ConfigurationProvider" />.
    /// </summary>
    public class AzureKeyVaultConfigurationProvider : ConfigurationProvider
    {
        private readonly IKeyVaultClient _client;
        private readonly IKeyVaultSecretManager _manager;
        private readonly string _vault;

        /// <summary>
        /// Creates a new instance of <see cref="AzureKeyVaultConfigurationProvider" />.
        /// </summary>
        /// <param name="client">The <see cref="KeyVaultClient" /> to use for retrieving values.</param>
        /// <param name="vault">Azure KeyVault uri.</param>
        /// <param name="manager"></param>
        public AzureKeyVaultConfigurationProvider(IKeyVaultClient client, string vault, IKeyVaultSecretManager manager)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _vault = vault ?? throw new ArgumentNullException(nameof(vault));
            _manager = manager;
        }

        /// <summary>
        /// Loads (or reloads) the data for this provider.
        /// </summary>
        public override void Load() => LoadAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        private async Task LoadAsync()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var secrets = await _client.GetSecretsAsync(_vault);
            do
            {
                foreach (var secretItem in secrets)
                {
                    if (!_manager.Load(secretItem))
                        continue;

                    var value = await _client.GetSecretAsync(secretItem.Id);
                    var key = _manager.GetKey(value);
                    data.Add(key, value.Value);
                }

                secrets = !string.IsNullOrEmpty(secrets.NextPageLink)
                    ? await _client.GetSecretsNextAsync(secrets.NextPageLink)
                    : null;
            }
            while (secrets != null);

            Data = data;
        }
    }
}