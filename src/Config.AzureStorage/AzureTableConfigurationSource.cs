using System;

using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.Extensions.Configuration.AzureStorage
{

    /// <summary>
    /// Provides an additional configuration provider for loading settings from Azure Storage.
    /// </summary>
    public class AzureTableConfigurationSource :
        IConfigurationSource
    {

        readonly CloudTableClient client;
        readonly string tableName;
        readonly string valueColumnName;
        readonly TimeSpan refreshInterval;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="tableName"></param>
        /// <param name="valueColumnName"></param>
        /// <param name="refreshInterval"></param>
        public AzureTableConfigurationSource(
            CloudTableClient client,
            string tableName,
            string valueColumnName,
            TimeSpan refreshInterval)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
            this.valueColumnName = valueColumnName ?? throw new ArgumentNullException(nameof(valueColumnName));
            this.refreshInterval = refreshInterval;
        }

        /// <summary>
        /// Builds the configuration provider.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return new AzureTableConfigurationProvider(
                client,
                tableName,
                valueColumnName,
                refreshInterval);
        }

    }

}
