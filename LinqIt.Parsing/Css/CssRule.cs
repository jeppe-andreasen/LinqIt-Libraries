using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Parsing.Html;
using LinqIt.Utils.Extensions;

namespace LinqIt.Parsing.Css
{
    public struct CssRule
    {
        private readonly List<CssSelectorStack> _selectorStacks;
        private readonly List<CssStatement> _statements;

        public CssRule(Token token)
        {
            _selectorStacks = new List<CssSelectorStack>();
            _statements = new List<CssStatement>();

            string data = token.ReadUntil('{');
            token.MovePast("{");
            
            foreach (var stack in data.Split(',').Select(CssSelectorStack.Parse).Where(stack => stack != null))
                _selectorStacks.Add(stack);
            
            var statements = token.ReadUntil('}').Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
            foreach (string statement in statements)
            {
                _statements.Add(new CssStatement(statement));
            }
            token.MovePast("}");
        }

        public IEnumerable<CssSelectorStack> Selectors { get { return _selectorStacks; } }

        public IEnumerable<CssStatement> Statements { get { return _statements; } }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine(_selectorStacks.ToSeparatedString(", "));
            result.AppendLine("{");
            foreach (var statement in _statements)
                result.AppendLine("\t" + statement);
            result.AppendLine("}");
            return result.ToString();
        }

        internal string GetDebuggingText()
        {
            var result = new StringBuilder();
            result.AppendLine(_selectorStacks.ToSeparatedString(", ", s => s.GetDebuggingString()));
            result.AppendLine("{");
            foreach (var statement in _statements)
                result.AppendLine("\t" + statement);
            result.AppendLine("}");
            return result.ToString();
        }

        public CssRuleMatch Match(HtmlTag tag)
        {
            var result = new CssRuleMatch(this);
            var selector = GetMatchingSelectors(tag).OrderByDescending(s => s.Specificity).FirstOrDefault();
            result.Success = selector != null;
            if (selector != null)
                result.Specificity = selector.Specificity;
            return result;
        }

        private IEnumerable<CssSelectorStack> GetMatchingSelectors(HtmlTag tag)
        {
            foreach (var stack in _selectorStacks)
            {
                if (stack.IsMatch(tag))
                    yield return stack;


                //var tmp = tag;
                //var selectorIndex = stack.Selectors.Count - 1;
                //while (selectorIndex >= 0 && tmp != null)
                //{
                //    if (stack.Selectors[selectorIndex].Matches(tmp))
                //        selectorIndex--;
                //    else if (selectorIndex == stack.Selectors.Count - 1)
                //    {
                //        break;
                //    }
                //    tmp = tmp.Parent as HtmlTag;
                //}
                //if (selectorIndex == -1)
                //{
                //    yield return stack;
                //}
            }
        }

        //public bool Matches(HtmlTag tag)
        //{
        //    return GetMatchingSelectors(tag).Any();
        //}

        
    }

    
}
