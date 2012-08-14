using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search.Utilities
{
    public class UrlCollection
    {
        private List<string> m_ProcessedUrls;
        private List<string> m_PendingUrls;

        public UrlCollection()
        {
            m_PendingUrls = new List<string>();
            m_ProcessedUrls = new List<string>();
        }

        public bool Push(string url)
        {
            lock (this)
            {
                if (!m_PendingUrls.Contains(url) && !m_ProcessedUrls.Contains(url))
                {
                    m_PendingUrls.Add(url);
                    return true;
                }
                return false;
            }
        }

        public void Push(IEnumerable<string> urls)
        {
            lock (this)
            {
                m_PendingUrls.AddRange(urls.Where(url => !m_PendingUrls.Contains(url) && !m_ProcessedUrls.Contains(url)));
            }
        }

        public string Pop()
        {
            lock (this)
            {
                if (m_PendingUrls.Any())
                {
                    string result = m_PendingUrls[0];
                    m_PendingUrls.RemoveAt(0);
                    m_ProcessedUrls.Add(result);
                    return result;
                }
                return null;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return !m_PendingUrls.Any();
            }
        }
    
    }
}
