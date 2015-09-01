using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace tablestoragetest
{
    public class Program
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        private CloudTableClient _tableClient;
        public IConfiguration Configuration { get; set; }

        public Program()
        {
            var builder = new ConfigurationBuilder("D:\\testproj\\src\\testproj");
            builder.AddUserSecrets();
            Configuration = builder.Build();

            // Retrieve the storage account from the connection string.
            _storageAccount = CloudStorageAccount.Parse(Configuration["Authentication:AzureStorageAccount:StorageConnectionString"]);
            // Create the table client.
            _tableClient = _storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            _table = _tableClient.GetTableReference("people");
            _table.CreateIfNotExistsAsync().Wait();
        } 
        public void retrievePeople()
        {

        }
        public void Main(string[] args)
        {
            Console.ReadLine();
        }
    }
}
