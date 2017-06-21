using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace ConsoleApplication
{
    public class EnvironmentSecretManager : IKeyVaultSecretManager
    {
        private readonly string _environmentPrefix;

        public EnvironmentSecretManager(string environment)
        {
            _environmentPrefix = environment + "-";
        }

        public bool Load(SecretItem secret)
        {
            return secret.Identifier.Name.StartsWith(_environmentPrefix);
        }

        public string GetKey(SecretBundle secret)
        {
            return secret.SecretIdentifier.Name
                                          .Substring(_environmentPrefix.Length)
                                          .Replace("--", ConfigurationPath.KeyDelimiter);
        }
    }
}
