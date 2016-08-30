// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly string _vaultUri;
        private readonly string _vaultName;
        private readonly IDictionary<string, string> _includedSecrets;
        private readonly IList<string> _excludedSecrets;

        /// <summary>
        /// Creates a new instance of <see cref="AzureKeyVaultConfigurationProvider"/>.
        /// </summary>
        /// <param name="client">The <see cref="KeyVaultClient"/> to use for retrieving values.</param>
        /// <param name="vaultUri">Azure KeyVault uri.</param>
        /// <param name="vaultName">An optional internal name for the vault.</param>
        /// <param name="includedSecrets">Allows to only load the specified secrets.</param>
        /// <param name="excludedSecrets">Allows to load all secrets except the ones specified here.</param>
        public AzureKeyVaultConfigurationProvider(
            IKeyVaultClient client, 
            string vaultUri, 
            string vaultName,
            IDictionary<string, string> includedSecrets,
            IList<string> excludedSecrets)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (vaultUri == null)
            {
                throw new ArgumentNullException(nameof(vaultUri));
            }
            _client = client;
            _vaultUri = vaultUri;
            _vaultName = vaultName;
            _includedSecrets = includedSecrets ?? new Dictionary<string, string>();
            _excludedSecrets = excludedSecrets ?? new List<string>();
        }

        /// <inheritdoc />
        public override void Load()
        {
            LoadAsync().GetAwaiter().GetResult();
        }

        private async Task LoadAsync()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (_includedSecrets.Count > 0)
            {
                await LoadIncludedOnlyAsync(data);
            }
            else
            {
                await LoadAllExceptExcludedAsync(data);
            }

            Data = data;
        }

        private async Task LoadIncludedOnlyAsync(IDictionary<string, string> data)
        {
            // By convention, if there's only a key in the dictionary, 
            // the actual Azure Key Vault secret name is equal to the internal name.
            foreach (var kvp in _includedSecrets)
            {
                if (string.IsNullOrWhiteSpace(kvp.Value))
                {
                    _includedSecrets[kvp.Key] = kvp.Key;
                }
            }

            // Swap dictionary to make sure every secret is loaded just once.
            // Before: InternalSecret -> AzureSecretName
            // After: AzureSecretName -> List<InternalSecret>
            var transformedSecrets = _includedSecrets
                .GroupBy(x => x.Value)
                .ToDictionary(key => key.Key, element => element.Select(y => y.Key));

            // loads all secrets in parallel.
            var tasks = transformedSecrets.Select(x => LoadSecretIntoDataAsync(x.Key, x.Value, data)).ToList();
            await Task.WhenAll(tasks);
        }

        private async Task  LoadSecretIntoDataAsync(
            string secretName, 
            IEnumerable<string> keys,
            IDictionary<string, string> data)
        {
            var secret = await _client.GetSecretAsync(_vaultUri, secretName);

            if (secret == null)
            {
                // TODO localize
                throw new InvalidOperationException($"Secret '{secretName}' does not exist in key vault.");
            }
            if (string.IsNullOrWhiteSpace(secret.Value))
            {
                // TODO localize
                throw new InvalidOperationException($"Secret '{secretName}' did not return a value.");
            }

            foreach (string key in keys)
            {
                data.Add(NormalizeKey(key), secret.Value);
            }
        }

        private async Task LoadAllExceptExcludedAsync(IDictionary<string, string> data)
        {
            var secrets = await _client.GetSecretsAsync(_vaultUri);
            do
            {
                foreach (var secretItem in secrets.Value)
                {
                    if (_excludedSecrets.Contains(secretItem.Identifier.Name))
                    {
                        continue;
                    }

                    var key = NormalizeKey(secretItem.Identifier.Name);
                    var value = await _client.GetSecretAsync(secretItem.Id);

                    data.Add(key, value.Value);
                }

                secrets = secrets.NextLink != null ?
                    await _client.GetSecretsNextAsync(secrets.NextLink) :
                    null;
            } while (secrets != null);
        }

        private string NormalizeKey(string key)
        {
            // Azure Key Vault does not allow the character ":" for secret names.
            key = key.Replace("__", ConfigurationPath.KeyDelimiter);
            
            if (!string.IsNullOrWhiteSpace(_vaultName))
            {
                key = _vaultName + ConfigurationPath.KeyDelimiter + key;
            }

            return key;
        }
    }
}