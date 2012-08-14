using System.Text.RegularExpressions;

namespace LinqIt.Cms.Data
{
    public class Html
    {
        #region Fields

        private readonly string _value;

        #endregion Fields

        #region Constructors

        public Html(string value)
        {
            _value = string.IsNullOrEmpty(value) ? string.Empty : value.Trim();
        }

        #endregion Constructors

        #region Properties

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(_value); }
        }

        #endregion Properties

        #region Methods

        public string AsText()
        {
            return _value.Replace("<br />", "\r\n");
        }

        public override string ToString()
        {
            return _value;
        }

        #endregion Methods

        public Text ToText()
        {
            if (string.IsNullOrEmpty(_value))
                return new Text(string.Empty);

            // Replace br with new line
            var text = Regex.Replace(_value, @"<br\s?/?>", "\r\n", RegexOptions.IgnoreCase);

            // Remove all html tags
            text = Regex.Replace(text, @"<(([^>""']*)|(""[^""]*"")|('[^']*'))*>", "");

            // Remove duplicate spaces
            text = Regex.Replace(text, @" +", " ", RegexOptions.Singleline);

            // Remove duplicate newlines)
            text = Regex.Replace(text, @"\r\n\s+", "\r\n", RegexOptions.Singleline);

            return new Text(text.Trim());
        }

        public Text GetExtract(int maxCharacters, bool revertToFullSentence)
        {
            var value = this.ToText().ToString();
            if (value.Length <= maxCharacters)
                return new Text(value);

            value = value.Substring(0, maxCharacters).Trim();
            if (revertToFullSentence)
            {
                var idx = value.LastIndexOf('.');
                if (idx > -1)
                    value = value.Substring(0, idx).Trim();
            }
            else
                value = value + "..";

            return new Text(value);
        }
    }
}