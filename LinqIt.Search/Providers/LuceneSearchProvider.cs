using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using LinqIt.Utils;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;

namespace LinqIt.Search.Providers
{
    public class LuceneSearchProvider : ISearchProvider
    {
        private FSDirectory _directory;
        private ServiceType _serviceType;
        private IndexWriter _writer;
        private IndexReader _reader;

        public void Open(string indexName, ServiceType serviceType)
        {
            try
            {
                var indexPath = System.IO.Path.Combine(ConfigurationManager.AppSettings["luceneIndexFolder"], indexName);
                _directory = FSDirectory.Open(new DirectoryInfo(indexPath));
                _serviceType = serviceType;

                if (serviceType == ServiceType.Read)
                {
                    _reader = IndexReader.Open(_directory, true);
                }
                else
                {
                    Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
                    _writer = new IndexWriter(_directory, analyzer, !System.IO.Directory.Exists(indexPath), IndexWriter.MaxFieldLength.LIMITED);
                }
            }
            catch(Exception exc)
            {
                Logging.Log(LogType.Error, "Unable to open index: " + indexName + ", ServiceType: " + serviceType, exc);
                throw;
            }
        }

        public void Close()
        {
            try
            {
                if (_serviceType == ServiceType.Read)
                {

                }
                else
                {
                    _writer.Close();
                }
                _directory.Close();
            }
            catch (Exception exc)
            {
                Logging.Log(LogType.Error, "Unable to close index: " + _directory + ", ServiceType: " + _serviceType, exc);
                throw;
            }
            
        }

        public SearchRecord[] Search(Query q, int skip, int take, out long totalResults)
        {
            totalResults = 0;
            var indexSearch = new IndexSearcher(_reader);
            SearchRecord[] result;
            try
            {
                var query = ResolveQuery(q);
                //var sort = new Sort(new SortField("date", SortField.LONG, true));
                //indexSearch.Search(query, new QueryWrapperFilter(query), 0, sort);
                
                var topDocuments = indexSearch.Search(query, _reader.MaxDoc());
                totalResults = topDocuments.totalHits;
                var hits = topDocuments.scoreDocs;
                
                result = hits.Skip(skip).Take(take).Select(hit => new LuceneRecord(indexSearch.Doc(hit.doc))).Cast<SearchRecord>().ToArray();
            }
            finally
            {
                indexSearch.Close();
            }
            return result;
        }

        public void ClearDatabase()
        {
            _writer.DeleteAll();
            _writer.Optimize();
        }

        public void UpsertRecords(IEnumerable<SearchRecord> records)
        {
            foreach (var record in records)
            {
                var query = new Lucene.Net.Search.BooleanQuery();
                query.Add(new Lucene.Net.Search.TermQuery(new Term("__id", record.Id)), BooleanClause.Occur.MUST);
                
                _writer.DeleteDocuments(query);
                Logging.Log(LogType.Info, "Deleted document: " + record.Id);
                _writer.AddDocument(((LuceneRecord)record).Document);
                Logging.Log(LogType.Info, "Added document: " + record.Id);
            }
        }

        public void RemoveRecord(string recordId)
        {
            var query = new Lucene.Net.Search.BooleanQuery();
            query.Add(new PrefixQuery(new Term("__id", recordId)), BooleanClause.Occur.MUST);
            _writer.DeleteDocuments(query);
        }

        #region Query Translations

        private Lucene.Net.Search.Query ResolveQuery(Query query)
        {
            var type = query.GetType();
            if (type == typeof(BooleanQuery))
                return GetBooleanQuery((BooleanQuery)query);
            if (type == typeof(ContainsQuery))
                return GetContainsQuery((ContainsQuery)query);
            if (type == typeof(NotQuery))
                return GetNotQuery((NotQuery)query);
            if (type == typeof(TermQuery))
                return GetTermQuery((TermQuery)query);
            if (type == typeof(RangeQuery))
                return GetRangeQuery((RangeQuery)query);
            if (type == typeof(WildCardQuery))
                return GetWildCardQuery((WildCardQuery)query);
            if (type == typeof(DateRangeQuery))
                return GetDateRangeQuery((DateRangeQuery) query);
            throw new NotImplementedException("Query Type " + type.Name + " has not been translated in " + GetType().Name);
        }

        private Lucene.Net.Search.Query GetBooleanQuery(BooleanQuery input)
        {
            var result = new Lucene.Net.Search.BooleanQuery();
            foreach (var query in input.Queries.Where(q => q != null))
                result.Add(ResolveQuery(query), input.Type == BooleanQueryType.And ? Lucene.Net.Search.BooleanClause.Occur.MUST : Lucene.Net.Search.BooleanClause.Occur.SHOULD);
            return result;
        }

        private static Lucene.Net.Search.Query GetContainsQuery(ContainsQuery input)
        {
            var result = new Lucene.Net.Search.BooleanQuery();
            foreach (var value in input.Values)
            {
                var wildcardQuery = new Lucene.Net.Search.WildcardQuery(new Lucene.Net.Index.Term(input.FieldName, "*" + value + "*"));
                result.Add(wildcardQuery, Lucene.Net.Search.BooleanClause.Occur.SHOULD);
            }
            return result;
        }

        private Lucene.Net.Search.Query GetNotQuery(NotQuery input)
        {
            var result = new Lucene.Net.Search.BooleanQuery();
            result.Add(ResolveQuery(input.Query), Lucene.Net.Search.BooleanClause.Occur.MUST_NOT);
            return result;
        }

        private static Lucene.Net.Search.Query GetTermQuery(TermQuery input)
        {
            return new Lucene.Net.Search.WildcardQuery(new Lucene.Net.Index.Term(input.FieldName, input.Value));
        }

        private static Lucene.Net.Search.Query GetRangeQuery(RangeQuery rangeQuery)
        {
            return new TermRangeQuery(rangeQuery.FieldName, rangeQuery.FromValue, rangeQuery.ToValue, rangeQuery.Inclusive, rangeQuery.Inclusive);
        }

        private static Lucene.Net.Search.Query GetWildCardQuery(WildCardQuery wildCardQuery)
        {
            return new Lucene.Net.Search.WildcardQuery(new Lucene.Net.Index.Term(wildCardQuery.FieldName, "*" + wildCardQuery.Value + "*"));
        }

        private static Lucene.Net.Search.Query GetDateRangeQuery(DateRangeQuery dateRangeQuery)
        {
            var from = dateRangeQuery.From.HasValue ? dateRangeQuery.From.Value : DateTime.MinValue;
            var to = dateRangeQuery.To.HasValue ? dateRangeQuery.To.Value : DateTime.MaxValue;
            return new TermRangeQuery(dateRangeQuery.FieldName, from.ToString("yyyyMMddHHmmss"), to.ToString("yyyyMMddHHmmss"), dateRangeQuery.IncludeLower, dateRangeQuery.IncludeUpper);
        }

        #endregion

        public SearchRecord CreateRecord(string id)
        {
            return new LuceneRecord(id);
        }

        public IEnumerable<SearchRecord> GetAllRecords()
        {
            var result = new List<SearchRecord>();
            var num = _reader.NumDocs();
            for (var i = 0; i < num; i++)
            {
                if (!_reader.IsDeleted(i))
                {
                    var document = _reader.Document(i);
                    result.Add(new LuceneRecord(document));
                }
            }
            return result;
        }

        public IEnumerable<string> GetAllTerms()
        {
            return _reader.GetFieldNames(IndexReader.FieldOption.ALL).ToArray();
        }
    }
}
