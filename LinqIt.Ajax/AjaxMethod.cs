using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Ajax
{
    public enum AjaxType { Async, Sync, Both };

    /// <summary>
    /// Use this attribute to signal that the method can be safely called from the Ajax Proxy
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AjaxMethod : System.Attribute
    {
        public AjaxMethod()
        {
            Type = AjaxType.Async;
        }

        public AjaxMethod(AjaxType type)
        {
            Type = type;
        }

        public AjaxType Type
        {
            get; set;
        }
    }
}
