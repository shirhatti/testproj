using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tablestoragetest.Models
{
    public class UrlEntity : TableEntity
    {
        public UrlEntity(string version, string identifier)
        {
            this.PartitionKey = version;
            this.RowKey = identifier;
        }

        public UrlEntity() { }

        public string fullPath { get; set; }
    }
}
