using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.Extensions.Configuration.AzureStorage
{

    /// <summary>
    /// Provides configuration options from an Azure Storage data table. Key values are given by the combination of
    /// PartitionKey and RowKey separated by a ':'. Values are stored within the Value column.
    /// </summary>
    public class AzureTableConfigurationProvider :
        ConfigurationProvider,
        IDisposable
    {

        readonly CloudTableClient client;
        readonly string tableName;
        readonly string valueColumnName;
        readonly TimeSpan refreshInterval;

        Task timer;
        CancellationTokenSource timerCts;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="tableName"></param>
        /// <param name="valueColumnName"></param>
        /// <param name="refreshInterval"></param>
        public AzureTableConfigurationProvider(
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
        /// Initiates a load of the configuration.
        /// </summary>
        public override void Load()
        {
            LoadAsync(CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Initiates an async load of the configuration.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task LoadAsync(CancellationToken cancellationToken)
        {
            try
            {
                var l = await GetConfiguration(cancellationToken);
                var d = l.ToDictionary(i => i.key, i => i.value, StringComparer.OrdinalIgnoreCase);

                // check for changes
                if (Data == null ||
                    Data.Count != d.Count ||
                    Data.Keys.Any(k => !d.ContainsKey(k) || !Equals(Data[k], d[k])))
                {
                    Data = d;
                    OnReload();
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                //  not much more we can do about it without a logger
                Trace.WriteLine(e.ToString());
            }

            // start reload timer
            StartTimer();
        }

        /// <summary>
        /// Periodically invoked to refresh the configuration.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async Task TimerAsync(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    await Task.Delay(refreshInterval, cancellationToken);
                    await LoadAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // ignore
                }
            }
        }

        /// <summary>
        /// Gets a value if it exists, or returns the default value.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue GetOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var result) ? result : default;
        }

        /// <summary>
        /// Gets the configuration available within the Azure Storage table.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async Task<(string key, string value)[]> GetConfiguration(CancellationToken cancellationToken)
        {
            var opt = new TableRequestOptions();
            var ctx = new OperationContext();

            var t = client.GetTableReference(tableName);
            await t.CreateIfNotExistsAsync(opt, ctx, cancellationToken);

            var q = new TableQuery();
            var c = default(TableContinuationToken);
            var l = new List<(string, string)>();

            do
            {
                // next set of results
                var r = await t.ExecuteQuerySegmentedAsync(q, c, opt, ctx, cancellationToken);

                // add results to list
                l.Capacity += r.Results.Count;

                // add items to list
                foreach (var item in r.Results.Select(i => (i.PartitionKey + ":" + i.RowKey, GetOrDefault(i.Properties, valueColumnName)?.StringValue)))
                    l.Add(item);

                // any more?
                c = r.ContinuationToken;
            }
            while (c != null);

            return l.ToArray();
        }

        /// <summary>
        /// Starts the refresh timer if required.
        /// </summary>
        void StartTimer()
        {
            // configure timer if refresh is enabled
            if (timer == null && refreshInterval > TimeSpan.Zero)
            {
                lock (this)
                {
                    if (timer == null)
                    {
                        timerCts = new CancellationTokenSource();
                        timer = Task.Run(() => TimerAsync(timerCts.Token));
                    }
                }
            }
        }

        /// <summary>
        /// Stops the refresh timer if it exists.
        /// </summary>
        void StopTimer()
        {
            try
            {
                if (timer != null)
                {
                    lock (this)
                    {
                        if (timer != null)
                        {
                            timerCts.Cancel();
                            timerCts = null;
                            timer.Wait();
                            timer = null;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }

        /// <summary>
        /// Disposes of the instance.
        /// </summary>
        public void Dispose()
        {
            StopTimer();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes the instance.
        /// </summary>
        ~AzureTableConfigurationProvider()
        {
            Dispose();
        }

    }

}
