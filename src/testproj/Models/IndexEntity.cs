using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace testsite.Models
{
    public class IndexEntity : TableEntity
    {
        public static string partitionKey = "default";
        public IndexEntity(string version)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = version;
        }

        public IndexEntity() { }

        public long index { get; set; }
    }
}
