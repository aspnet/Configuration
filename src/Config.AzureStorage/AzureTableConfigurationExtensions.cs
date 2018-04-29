using System;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.Extensions.Configuration.AzureStorage
{

    /// <summary>
    /// Provides methods for Azure Data Tables and Microsoft Extensions Configuration.
    /// </summary>
    public static class AzureTableConfigurationExtensions
    {

        const string DefaultTableName = "config";
        const string DefaultValueColumnName = "Value";

        /// <summary>
        /// Adds an Azure Data Table as a configuration source.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="client"></param>
        /// <param name="tableName"></param>
        /// <param name="valueColumnName"></param>
        /// <param name="refreshInterval"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddAzureDataTable(
            this IConfigurationBuilder builder,
            CloudTableClient client,
            string tableName = DefaultTableName,
            string valueColumnName = DefaultValueColumnName,
            TimeSpan? refreshInterval = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Add(
                new AzureTableConfigurationSource(
                    client ?? throw new ArgumentNullException(nameof(client)),
                    tableName,
                    valueColumnName,
                    refreshInterval ?? TimeSpan.FromMinutes(15)));
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="account"></param>
        /// <param name="tableName"></param>
        /// <param name="valueColumnName"></param>
        /// <param name="refreshInterval"></param>
        public static IConfigurationBuilder AddAzureDataTable(
            this IConfigurationBuilder builder,
            CloudStorageAccount account,
            string tableName = DefaultTableName,
            string valueColumnName = DefaultValueColumnName,
            TimeSpan? refreshInterval = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            return AddAzureDataTable(
                builder,
                account.CreateCloudTableClient(),
                tableName,
                valueColumnName,
                refreshInterval);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <param name="valueColumnName"></param>
        /// <param name="refreshInterval"></param>
        public static IConfigurationBuilder AddAzureDataTable(
            this IConfigurationBuilder builder,
            string connectionString,
            string tableName = DefaultTableName,
            string valueColumnName = DefaultValueColumnName,
            TimeSpan? refreshInterval = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            return AddAzureDataTable(
                builder,
                CloudStorageAccount.Parse(connectionString),
                tableName,
                valueColumnName,
                refreshInterval);
        }

    }

}
