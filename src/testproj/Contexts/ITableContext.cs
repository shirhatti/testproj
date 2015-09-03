using System.Collections.Generic;
using System.Threading.Tasks;

namespace testproj.Contexts
{
    public interface ITableContext
    {
        void Configure();
        Task<Dictionary<string, string>> RetrieveUrl();
        Task<string> ResolverUrl(string identifier);
        Task<string> CreateUrl(string fullUrl);
    }
}