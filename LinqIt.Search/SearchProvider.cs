using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    public interface ISearchProvider
    {
        SearchRecord[] Search(Query query, int skip, int take, out long totalResults);

        void ClearDatabase();

        void UpsertRecords(IEnumerable<SearchRecord> records);

        void RemoveRecord(string recordId);

        SearchRecord CreateRecord(string id);

        IEnumerable<SearchRecord> GetAllRecords();

        IEnumerable<string> GetAllTerms();

        void Close();

        void Open(string indexName, ServiceType write);
    }
}
