//namespace LinqIt.Cms.Configuration
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Xml;

//    /// <summary>
//    /// Base class for Settings wrappers
//    /// </summary>
//    public abstract class SettingsBase
//    {
//        #region Fields

//        private readonly Dictionary<string, string> _settings;

//        #endregion Fields

//        #region Constructors

//        internal SettingsBase()
//        {
//        }

//        internal SettingsBase(XmlElement solutionSettings)
//        {
//            XmlElement root = solutionSettings[GetType().Name];
//            if (root != null)
//            {
//                this._settings = root.ChildNodes.Cast<XmlElement>().ToDictionary(e => e.Name, e => e.InnerText);
//            }
//        }

//        #endregion Constructors

//        #region Methods

//        protected void AssignDefault(string key, string value)
//        {
//            if (!this._settings.ContainsKey(key))
//                this._settings.Add(key, value);
//        }

//        protected string GetSetting(string key)
//        {
//            if (!this._settings.ContainsKey(key))
//                throw new IndexOutOfRangeException("The setting " + GetType().Name + "." + key + " has not been initialized.");
//            return this._settings[key];
//        }

//        #endregion Methods
//    }
//}