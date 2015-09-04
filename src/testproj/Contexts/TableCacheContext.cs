using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// TODO there is no cache invalidation implemented
namespace testproj.Contexts
{
    public class TableCacheContext : ITableContext
    {
        private AzureTableContext _azureTable;
        private ConcurrentDictionary<string, string> _cache;
        private bool _cacheInvalid = false;

        public TableCacheContext(string DbConnectionString, string version)
        {
            _azureTable = new AzureTableContext(DbConnectionString, version);
        }

        public async void Configure()
        {
            _azureTable.Configure();
            var dictonary = await _azureTable.RetrieveUrl();
            _cache = new ConcurrentDictionary<string, string>(dictonary);
        }

        public async Task<Dictionary<string, string>> RetrieveUrl() 
        {
            if (!_cacheInvalid)
            {
                return new Dictionary<string, string>(_cache);
            }
            return await _azureTable.RetrieveUrl();
        }

        public async Task<string> ResolverUrl(string identifier)
        {
            if (!_cacheInvalid)
            {
                if (_cache.ContainsKey(identifier))
                {
                    return _cache[identifier];
                }
            }
            return await _azureTable.ResolverUrl(identifier);
        }

        public async Task<string> CreateUrl(string fullUrl)
        {

            var status = await _azureTable.CreateUrl(fullUrl);
            if (status == "_failed")
            {
                return status;
            }
            else
            {
                _cache.TryAdd(status, fullUrl);
                return status;
            }
        }

    }
}
