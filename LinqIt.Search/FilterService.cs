using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinqIt.Search.Configuration;
using LinqIt.Search.Filters;
using LinqIt.Search.Utilities;

namespace LinqIt.Search
{
    public class FilterService
    {
        private readonly List<Filter> _filters;

        public FilterService(LinqItSearchConfigurationSection configuration)
        {
            var assemblies = new Dictionary<string, Assembly>();
            _filters = configuration.FilterConfigurations.Cast<IFilterConfiguration>().Select(c => LoadFilter(c, assemblies)).ToList();
        }

        private static Filter LoadFilter(IFilterConfiguration c, Dictionary<string, Assembly> assemblies)
        {
            var assemblyType = new AssemblyType(c.Type);

            Assembly assembly;
            if (assemblies.ContainsKey(assemblyType.AssemblyName))
                assembly = assemblies[assemblyType.AssemblyName];
            else
            {
                assembly = Assembly.Load(assemblyType.AssemblyName);
                assemblies.Add(assemblyType.AssemblyName, assembly);
            }
            var filter = assemblyType.Instantiate<Filter>(assembly);
            filter.Initialize(c);
            return filter;
        }

        internal FilterResult Process(CrawlData dataSet)
        {
            var result = new FilterResult();
            var filter = _filters.Where(f => f.CanProcess(dataSet)).FirstOrDefault();
            if (filter == null)
            {
                result.Message = "No appropriate filter was found for mimetype : " + dataSet.MimeType;
                return result;
            }
            try
            {
                if (!filter.Process(dataSet))
                    result.Message = "The ifilter was unsuccessful.";
                //else if (dataSet.FilteredContent == null)
                //    result.Message = "The ifilter did not extract any text";
                else
                    result.Success = true;
            }
            catch (Exception exc)
            {
                result.Message = exc.Message;
                result.Exception = exc;
            }
            return result;
        }
    }
}
