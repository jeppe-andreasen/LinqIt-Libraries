using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;

namespace LinqIt.Cms.Data.DataIterators
{
    public class WebConfigTransform
    {
        private static readonly Regex _transformRegex = new Regex(@"^(?<fn>[a-zA-Z]+)(?:\((?<params>[^)]*)\))?$");
        private static readonly Regex _locatorRegex = new Regex(@"^(?<fn>[a-zA-Z]+)(?:\((?<params>[^)]*)\))?$");
        private static readonly Regex _namespaceRegex = new Regex(@"\s?(?:(?<ns>xmlns\:xdt)|(?<nsv>xdt\:[a-z]+))=""[^""]*""", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly XNamespace _xdt = "http://schemas.microsoft.com/XML-Document-Transform";
        private XDocument _original;

        public string Process(string configurationName)
        {
            _original = XDocument.Load(HttpContext.Current.Server.MapPath("~/Web.config"));
            var transform = XDocument.Load(HttpContext.Current.Server.MapPath("~/Web." + configurationName + ".config"));
            foreach (var element in transform.Elements())
                ProcessElement(element);
            return _original.ToString(SaveOptions.None);
        }

        private void ProcessElement(XElement element)
        {
            var attr = element.Attribute(_xdt + "Transform");
            if (attr != null)
            {
                var value = (string)attr;
                var match = _transformRegex.Match(value);
                if (!match.Success)
                    throw new ApplicationException("Unable to parse function: " + value);

                switch (match.Groups["fn"].Value.ToLower())
                {
                    case "setattributes":
                        SetAttributes(element);
                        break;
                    case "removeattributes":
                        RemoveAttributes(element, match.Groups["params"].Value.Split(','));
                        break;
                    case "replace":
                        Replace(element);
                        break;
                    default:
                        throw new NotImplementedException("Transform function not implemented: " + match.Groups["fn"].Value);
                }

            }

            foreach (var child in element.Elements())
                ProcessElement(child);
        }

        private void Replace(XElement element)
        {
            var originalElement = GetOriginalElement(element);
            if (originalElement == null)
                return;

            var oldXml = element.ToString(SaveOptions.None);
            var cleanedXml = _namespaceRegex.Replace(oldXml, "");
            var newElement = XElement.Parse(cleanedXml);
            originalElement.AddBeforeSelf(newElement);
            originalElement.Remove();
        }

        private void SetAttributes(XElement element)
        {
            var originalElement = GetOriginalElement(element);
            if (originalElement == null)
                return;

            foreach (var attr in element.Attributes())
            {
                if (attr.Name.NamespaceName != "")
                    continue;
                originalElement.SetAttributeValue(attr.Name, (string)attr);
            }
        }

        private void RemoveAttributes(XElement element, IEnumerable<string> attributes)
        {
            var originalElement = GetOriginalElement(element);
            if (originalElement == null) 
                return;
            foreach (var attr in attributes.Select(name => originalElement.Attribute(name)).Where(attr => attr != null))
                attr.Remove();
        }

        private XElement GetOriginalElement(XElement transformElement)
        {
            var path = GetPath(transformElement);
            var elements = _original.XPathSelectElements(path).ToArray();
            var locator = transformElement.Attribute(_xdt + "Locator");
            if (elements.Any() && locator != null)
            {
                Match match = _locatorRegex.Match((string) locator);
                if (match.Success)
                {
                    switch (match.Groups["fn"].Value.ToLower())
                    {
                        case "match":
                            var keyAttr = match.Groups["params"].Value;
                            var key = (string)transformElement.Attribute(keyAttr);
                            elements = elements.Where(e => (string) e.Attribute(keyAttr) == key).ToArray();
                            break;
                    }
                }
            }
            return elements.Length != 1 ? null : elements[0];
        }

        private static string GetPath(XElement element)
        {
            if (element.Parent == null)
                return element.Name.ToString();
            
            return GetPath(element.Parent) + "/" + element.Name;
        }
    }
}
