namespace LinqIt.Cms.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    [Serializable]
    public class Id : IComparable
    {
        #region Fields

        private Guid? _guid;
        private int? _int;

        public static Id Null
        {
            get
            {
                return new Id("");
            }
        }

        #endregion Fields

        #region Constructors

        public Id()
        {
            
        }

        public Id(int id)
        {
            _int = id;
        }

        public Id(Guid id)
        {
            _guid = id;
        }

        public Id(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int i;
                Guid guid;
                if (int.TryParse(id, out i)) 
                    _int = i;
                else if (Guid.TryParse(id, out guid)) 
                    _guid = new Guid(id);
            }
        }

        public string AsClientId
        {
            get
            {
                return "_" + Regex.Replace(this.ToString(), "[^a-zA-Z0-9]", "");
            }
        }

        #endregion Constructors

        #region Properties

        public Guid GuidValue
        {
            get
            {
                return _guid != null ? _guid.Value : Guid.Empty;
            }
        }

        public int IntValue
        {
            get
            {
                return _int != null ? _int.Value : default(int);
            }
        }

        public bool IsNull
        {
            get
            {
                return _guid == null && _int == null;
            }
        }

        #endregion Properties

        #region Methods

        public static bool operator !=(Id a, Id b)
        {
            return !(a == b);
        }

        public static bool operator ==(Id a, Id b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            if (a._int.HasValue)
                return a._int == b._int;
            else if (a._guid.HasValue)
                return a._guid == b._guid;

            return false;
        }

        public int CompareTo(object obj)
        {
            return GetHashCode().CompareTo(obj.GetHashCode());
        }

        public override bool Equals(System.Object obj)
        {
            return Equals(obj as Id);
        }

        public bool Equals(Id id)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(this, id))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ((object)id == null)
            {
                return false;
            }

            if (_int.HasValue)
                return _int == id._int;
            else if (_guid.HasValue)
                return _guid == id._guid;

            return false;
        }

        public override int GetHashCode()
        {
            if (_int.HasValue)
                return _int.GetHashCode();
            else if (_guid.HasValue)
                return _guid.GetHashCode();
            else
            {
                return "".GetHashCode();
            }
        }

        public override string ToString()
        {
            if (_int.HasValue)
                return _int.Value.ToString();
            if (_guid.HasValue)
                return _guid.Value.ToString("B").ToUpper();
            return string.Empty;
        }

        public static bool HasValue(Id id)
        {
            return id != null && !id.IsNull;
        }

        #endregion Methods
    }
}