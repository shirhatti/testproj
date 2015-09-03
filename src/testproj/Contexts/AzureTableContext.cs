using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using testsite.Helper;
using testsite.Models;

namespace testproj.Contexts
{
    public class AzureTableContext : ITableContext
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _urltable;
        private CloudTableClient _tableClient;
        private CloudTable _indextable;
        private string _version;

        public AzureTableContext(string DbConnectionString, string version)
        {
            _version = version;

            // Retrieve the storage account from the connection string.
            _storageAccount = CloudStorageAccount.Parse(DbConnectionString);
            
        }

        public void Configure()
        {
            // Create the table client.
            _tableClient = _storageAccount.CreateCloudTableClient();
            _urltable = _tableClient.GetTableReference("url");
            _indextable = _tableClient.GetTableReference("index");

            // Create the table if it doesn't exist
            _urltable.CreateIfNotExistsAsync().Wait();
            _indextable.CreateIfNotExistsAsync().Wait();

            // Create index if it doesn't exist
            createIndexIfNotExist().Wait();
        }

        public async Task<string> ResolverUrl(string identifier)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<UrlEntity>(_version, identifier);

            // Execute the retrieve operation.
            TableResult retrievedResult = await _urltable.ExecuteAsync(retrieveOperation);
            if (retrievedResult.HttpStatusCode == 200)
            {
                return ((UrlEntity)retrievedResult.Result).fullPath;
            }
            else
                return "_failed";

        }
        public async Task<string> CreateUrl(string fullUrl)
        {
            long index = await getIndexAndIncrement();
            if (index == -1)
            {
                return "_failed";
            }
            string identifier = Base62Encode.Encode(index);
            var url = new UrlEntity(_version, identifier);
            url.fullPath = fullUrl;
            TableOperation insertOperation = TableOperation.Insert(url);
            await _urltable.ExecuteAsync(insertOperation);
            return identifier;
        }

        public async Task<Dictionary<string, string>> RetrieveUrl()
        {
            // Construct the query operation for all customer entities
            TableQuery<UrlEntity> query = new TableQuery<UrlEntity>();
            TableContinuationToken continuationToken = null;

            var ret = new Dictionary<string, string>();

            do
            {
                // Retrieve a segment
                TableQuerySegment<UrlEntity> tableQueryResult = await _urltable.ExecuteQuerySegmentedAsync(query, continuationToken);

                // Assign the new continuation token
                continuationToken = tableQueryResult.ContinuationToken;

                foreach (UrlEntity url in tableQueryResult.Results)
                {
                    if (url != null)
                    {
                        ret.Add(url.RowKey, url.fullPath);
                    }
                }
            } while (continuationToken != null);
            return ret;
        }

        private async Task createIndexIfNotExist()
        {
            long ret = await getIndex();
            if (ret == -1)
            {
                IndexEntity i = new IndexEntity(_version);
                i.index = 0;

                TableOperation insertOperation = TableOperation.Insert(i);
                await _indextable.ExecuteAsync(insertOperation);
            }
        }
        private async Task<long> getIndexAndIncrement()
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<IndexEntity>("default", _version);

            // Execute the retrieve operation.
            TableResult retrievedResult = await _indextable.ExecuteAsync(retrieveOperation);

            var result = (IndexEntity)retrievedResult.Result;
            // Print the phone number of the result.
            if (result != null)
            {
                result.index = result.index + 1;
                TableOperation insertOperation = TableOperation.InsertOrReplace(result);
                await _indextable.ExecuteAsync(insertOperation);
                return result.index;
            }
            else
                return -1;
        }

        private async Task<long> getIndex()
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<IndexEntity>("default", _version);

            // Execute the retrieve operation.
            TableResult retrievedResult = await _indextable.ExecuteAsync(retrieveOperation);

            var result = (IndexEntity)retrievedResult.Result;
            // Print the phone number of the result.
            if (result != null)
            {
                return result.index;
            }
            else
                return -1;
        }
    }
}
