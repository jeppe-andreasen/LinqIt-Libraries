namespace LinqIt.Cms.Data
{
    public class Text
    {
        #region Fields

        private readonly string _value;

        #endregion Fields

        #region Constructors

        public Text(string value)
        {
            _value = value;
        }

        #endregion Constructors

        #region Properties

        public string AsHtml
        {
            get { return ToHtml().ToString(); }
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(_value); }
        }

        #endregion Properties

        #region Methods

        public Html ToHtml()
        {
            return string.IsNullOrEmpty(_value) ? new Html("") : new Html(_value.Replace("\n", "<br />").Replace("\r", ""));
        }

        public override string ToString()
        {
            return _value;
        }

        #endregion Methods
    }
}