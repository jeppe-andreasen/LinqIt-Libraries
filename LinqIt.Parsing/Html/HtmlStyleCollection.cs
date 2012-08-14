using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Utils.Collections;

namespace LinqIt.Parsing.Html
{
    public class HtmlStyleCollection : CaseInvariantNameValueCollection
    {
        public void Parse(string style)
        {
            Clear();
            if (string.IsNullOrEmpty(style))
                return;
            foreach (var stylePair in style.Split(';').Select(p => p.Trim()))
            {
                if (string.IsNullOrEmpty(stylePair))
                    continue;
                var pair = stylePair.Split(':');
                if (pair.Length != 2)
                    continue;
                this[pair[0].Trim()] = pair[1].Trim();
            }
        }

        public void Append(HtmlStyleCollection styles, bool reportUpdate)
        {
            BeginUpdate();
            foreach (var key in styles.Keys)
                Append(key, styles[key]);
            EndUpdate(reportUpdate);
        }
    }
}
