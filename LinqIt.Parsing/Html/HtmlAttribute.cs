using System.Linq;
using System.Text.RegularExpressions;

namespace LinqIt.Parsing.Html
{
    //public class HtmlAttribute
    //{
    //    private string _value;

    //    public HtmlAttribute(string name, string value)
    //    {
    //        Name = name;
    //        Value = value;
    //    }

    //    public string Name { get; set; }

    //    public string Value
    //    {
    //        get { return _value; }
    //        set { _value = Regex.Replace(value, @"[\n]", " "); }
    //    }

    //    public override string ToString()
    //    {
    //        if (string.IsNullOrEmpty(Value))
    //            return Name;
    //        else
    //        {
    //            var c = Value.Contains('"') ? '\'' : '"';
    //            return string.Format("{0}={1}{2}{1}", Name, c, Value);
    //        }
    //    }
    //}
}