using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using LinqIt.Search.Configuration;

namespace LinqIt.Search
{
    public class SearchService : IDisposable
    {
        private readonly LinqItSearchConfigurationSection _configuration;
        private readonly ISearchProvider _provider;

        public SearchService(string indexName)
        {
            _configuration = (LinqItSearchConfigurationSection)ConfigurationManager.GetSection("LinqItSearch");
            _provider = (ISearchProvider)Activator.CreateInstance(Type.GetType(_configuration.Provider));
            _provider.Open(indexName, ServiceType.Read);
        }

        public SearchResult Search(QueryList queryList, int skip, int take)
        {
            return Search(queryList.AsQuery, skip, take);
        }

        public SearchResult Search(Query query, int skip, int take)
        {
            long totalResults;
            var records = _provider.Search(query, skip, take, out totalResults);
            return new SearchResult(records, skip, take, totalResults);
        }

        public IEnumerable<string> GetAllTerms()
        {
            return _provider.GetAllTerms();
        }

        public IEnumerable<SearchRecord> GetAllRecords()
        {
            return _provider.GetAllRecords();

        }

        public void Dispose()
        {
            _provider.Close();
        }
    }
}
