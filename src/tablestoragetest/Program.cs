using System;
using System.Threading.Tasks;
using Microsoft.Framework.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using tablestoragetest.Models;

namespace tablestoragetest
{
    public class Program
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _urltable;
        private CloudTableClient _tableClient;
        private CloudTable _indextable;
        private string _version;

        public IConfiguration Configuration { get; set; }

        public Program()
        {
            var builder = new ConfigurationBuilder("D:\\testproj\\src\\testproj");
            builder.AddUserSecrets();
            Configuration = builder.Build();

            _version = Configuration["Authentication:AzureStorageAccount:version"];
            // Retrieve the storage account from the connection string.
            _storageAccount = CloudStorageAccount.Parse(Configuration["Authentication:AzureStorageAccount:StorageConnectionString"]);
            // Create the table client.
            _tableClient = _storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            _urltable = _tableClient.GetTableReference("url");
            _urltable.CreateIfNotExistsAsync();


            // Create the table if it doesn't exist.
            _indextable = _tableClient.GetTableReference("index");
            _indextable.CreateIfNotExistsAsync();
        }
        public UrlEntity retrieveUrl(string version, string identifier)
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<UrlEntity>(version, identifier);

            // Execute the retrieve operation.
            Task<TableResult> retrievedResult = _urltable.ExecuteAsync(retrieveOperation);

            // Print the phone number of the result.
            if (retrievedResult.Result.Result != null)
            {
                Console.WriteLine(((UrlEntity)retrievedResult.Result.Result).fullPath);
                return (UrlEntity)retrievedResult.Result.Result;
            }
            else
            {
                Console.WriteLine("No URL associated with that identifier could be found");
                return null;
            }
                

        }
        public async Task retrieveUrl()
        {
            // Construct the query operation for all customer entities
            TableQuery<UrlEntity> query = new TableQuery<UrlEntity>();
            TableContinuationToken continuationToken = null;

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
                        Console.WriteLine(url.RowKey + "\t" + url.PartitionKey + "\t" + url.fullPath);
                    }
                }
            } while (continuationToken != null);
        }

        public async void retrieveIndex()
        {
            // Construct the query operation for all customer entities
            TableQuery<IndexEntity> query = new TableQuery<IndexEntity>();
            TableContinuationToken continuationToken = null;

            do
            {
                // Retrieve a segment
                TableQuerySegment<IndexEntity> tableQueryResult = await _indextable.ExecuteQuerySegmentedAsync(query, continuationToken);

                // Assign the new continuation token
                continuationToken = tableQueryResult.ContinuationToken;

                foreach (IndexEntity index in tableQueryResult.Results)
                {
                    if (index != null)
                    {
                        Console.WriteLine(index.RowKey + "\t" + index.PartitionKey + "\t" + index.index);
                    }
                }
            } while (continuationToken != null);
        }

        public async void  createUrl(string identifier, string fullPath)
        {
            UrlEntity url = new UrlEntity("v1", identifier);
            url.fullPath = fullPath;

            TableOperation insertOperation = TableOperation.Insert(url);
            await _urltable.ExecuteAsync(insertOperation);
        }

        public async Task<long> createIndexIfNotExist(string version)
        {
            long ret = await getIndex(version);
            if (ret == -1)
            {
                IndexEntity i = new IndexEntity(version);
                i.index = 0;

                TableOperation insertOperation = TableOperation.Insert(i);
                await _indextable.ExecuteAsync(insertOperation);
                Console.WriteLine("Created index for version " + version);
                return 0;
            }
            Console.WriteLine("Index already exists for version" + version);
            return ret;
        }

        public async void deleteUrl(string version, string identifier)
        {
            UrlEntity url = retrieveUrl(version, identifier);
            if (url != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(url);
                await _urltable.ExecuteAsync(deleteOperation);             
                    Console.WriteLine(url.RowKey + "\t" + url.PartitionKey + "\t" + url.fullPath + " has been deleted");

            }
            Console.WriteLine("Url does not exist");

        }

        public async Task<long> getIndex(string version)
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<IndexEntity>("default", version);

            // Execute the retrieve operation.
            TableResult retrievedResult = await _indextable.ExecuteAsync(retrieveOperation);

            var result = (IndexEntity)retrievedResult.Result;
            // Print the phone number of the result.
            if (result != null)
            {
                Console.WriteLine(result.index);
                return result.index;
            }

            else
                Console.WriteLine("Index does not exist for " + version);
            return -1;
        }
        public long getIndexAndIncrement(string version)
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<IndexEntity>("default", version);

            // Execute the retrieve operation.
            Task<TableResult> retrievedResult = _indextable.ExecuteAsync(retrieveOperation);

            var result = (IndexEntity)retrievedResult.Result.Result;
            // Print the phone number of the result.
            if (result != null)
            {
                Console.WriteLine(result.index);
                return result.index;

                // TODO add the increment part of the code

            }
            else
                return -1;
        }

        public void Main(string[] args)
        {
            createUrl("msft", "https://www.microsoft.com");
            Console.ReadLine();
        }
    }
}
