using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LinqIt.Search.Arguments
{
    public class RequestEventArgs : EventArgs
    {
        public RequestEventArgs(HttpWebRequest request)
        {
            Request = request;
            IsCancelled = false;
        }

        public HttpWebRequest Request { get; private set; }

        public bool IsCancelled { get; set; }
    }
}
