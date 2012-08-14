using LinqIt.Utils.Caching;
using LinqIt.Utils.Extensions;

namespace LinqIt.Cms.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;

    using LinqIt.Cms.Data;
    
    

    public class CodeGenerationConfiguration
    {
        #region Fields

        private readonly string _name;
        private readonly CodeGenerationSettings _settings;
        private readonly Dictionary<string, TemplateSetting> _templateSettings;

        #endregion Fields

        #region Constructors

        internal CodeGenerationConfiguration(CodeGenerationSettings settings, string name, XmlElement root)
        {
            this._name = name;
            this._settings = settings;
            this._templateSettings = new Dictionary<string, TemplateSetting>();
            InitializeSettings();

            if (root != null)
            {
                IncludePathRoot = root.SelectSingleNode("IncludePaths/@root").Value;
                TrimTemplatesByIncludePaths(root);
                TrimTemplatesByExcludePaths(root);
                ApplyNameSpaces();
                LoadDefaultFieldSettings(root);
                LoadBaseClasses(root);
                OverrideByCustomSettings(root);
                RemoveSystemFields();
                ApplyPropertyNames();
            }
        }

        

        #endregion Constructors

        #region Properties

        public string IncludePathRoot
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public IEnumerable<TemplateSetting> GetTemplateSettings()
        {
            return this._templateSettings.Values;
        }

        internal static string GetCsName(string name)
        {
            string value = Regex.Replace(name, "[^a-zA-Z0-9_]", "");
            return char.ToUpper(value[0]) + value.Substring(1);
        }

        private void ApplyNameSpaces()
        {
            foreach (var template in this._templateSettings.Values)
            {
                var parts = template.Path.Substring(this.IncludePathRoot.Length).Split('/').Select(GetCsName).ToArray();
                template.NameSpace = parts.Take(parts.Length - 1).ToSeparatedString(".");
            }
        }

        private void ApplyPropertyNames()
        {
            foreach (var template in this._templateSettings.Values)
            {
                foreach (var field in template.FieldSettings.Values)
                {
                    string propertyName = GetCsName(field.Name);
                    if (propertyName == template.ClassName)
                        propertyName += "Field";
                    field.PropertyName = propertyName;
                }
            }
        }

        private string GetFullClassNameFromTemplatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (path.StartsWith("/") && _templateSettings.ContainsKey(path))
            {
                var template = _templateSettings[path];
                return (template.NameSpace + "." + template.ClassName).TrimStart('.');
            }
            try
            {
                var id = new Id(path);
                var template = _templateSettings.Values.Where(t => t.Id == id).FirstOrDefault();
                if (template != null)
                    return (template.NameSpace + "." + template.ClassName).TrimStart('.');
            }
            catch (Exception)
            {
            }
            return path;
        }

        private void InitializeSettings()
        {
            foreach (var template in CmsService.Instance.GetTemplates())
            {
                TemplateSetting templateSetting = new TemplateSetting(template);
                this._templateSettings.Add(templateSetting.Path, templateSetting);
            }
        }

        private void LoadBaseClasses(XmlElement root)
        {
            foreach (XmlElement element in root.SelectNodes("BaseClasses/BaseClass"))
            {
                bool ommitAll = false;
                var customFields = (XmlElement)element.SelectSingleNode("CustomFields");
                if (customFields != null && customFields.GetAttribute("ommitAll") == "true")
                    ommitAll = true;

                string[] fieldNames = element.SelectNodes("CustomFields/Field").Cast<XmlElement>().Select(e => e.GetAttribute("name")).ToArray();
                string[] searchPaths = element.SelectNodes("SearchPaths/Path").Cast<XmlElement>().Select(e => e.InnerText).ToArray();
                foreach (string key in this._templateSettings.Keys.Where(k => searchPaths.Where(s => k.IsWildcardMatch(s)).Any()))
                {
                    TemplateSetting setting = this._templateSettings[key];
                    setting.BaseClass = element.GetAttribute("type");
                    if (ommitAll)
                        setting.FieldSettings.Clear();
                    else
                    {
                        foreach (string fieldName in fieldNames)
                        {
                            if (setting.FieldSettings.ContainsKey(fieldName))
                                setting.FieldSettings.Remove(fieldName);
                        }
                    }
                }
            }
        }

        private void LoadDefaultFieldSettings(XmlElement root)
        {
            Dictionary<string, string> csTypes = root.SelectNodes("DefaultSettings/FieldSetting").Cast<XmlElement>().ToDictionary(e => e.GetAttribute("cmstype"), e => e.GetAttribute("cstype"));
            Dictionary<string, string> methods = root.SelectNodes("DefaultSettings/FieldSetting").Cast<XmlElement>().ToDictionary(e => e.GetAttribute("cmstype"), e => e.GetAttribute("method"));

            foreach (var templateSetting in this._templateSettings.Values)
                foreach (var fieldSetting in templateSetting.FieldSettings.Values)
                {
                    if (csTypes.ContainsKey(fieldSetting.CmsType))
                    {

                        string csType = csTypes[fieldSetting.CmsType];
                        Match enumTypeTableMatch = Regex.Match(csType, @"Entity\[(?<typetable>[^\]]+)\]");
                        if (csType == "?")
                        {
                            fieldSetting.CsType = GetFullClassNameFromTemplatePath(fieldSetting.ContainedType);
                            fieldSetting.Method = "GetValue<" + GetFullClassNameFromTemplatePath(fieldSetting.ContainedType) + ">";
                        }
                        else if (csType == "?[]")
                        {
                            fieldSetting.CsType = "IEnumerable<" + GetFullClassNameFromTemplatePath(fieldSetting.ContainedType) + ">";
                            fieldSetting.Method = "GetListValue<" + GetFullClassNameFromTemplatePath(fieldSetting.ContainedType) + ">";
                        }
                        else if (enumTypeTableMatch.Success)
                        {
                            fieldSetting.TypeTable = enumTypeTableMatch.Groups["typetable"].Value;
                            string innerType = !string.IsNullOrEmpty(fieldSetting.ContainedType) ? GetFullClassNameFromTemplatePath(fieldSetting.ContainedType) : "Entity";
                            fieldSetting.CsType = "IEnumerable<" + innerType + ">";
                            fieldSetting.Method = "GetDescendantsOfType<" + innerType + "," + fieldSetting.TypeTable +  ">";
                        }
                        else
                        {
                            switch (csType)
                            {
                                case "Entity":
                                    fieldSetting.CsType = !string.IsNullOrEmpty(fieldSetting.ContainedType) ? GetFullClassNameFromTemplatePath(fieldSetting.ContainedType) : csType;
                                    fieldSetting.Method = "GetEntity<" + fieldSetting.CsType + ">";
                                    break;
                                case "Entity[]":
                                    string containedType = !string.IsNullOrEmpty(fieldSetting.ContainedType) ? GetFullClassNameFromTemplatePath(fieldSetting.ContainedType) : "Entity";
                                    fieldSetting.CsType = "IEnumerable<" + containedType + ">";
                                    fieldSetting.Method = "GetEntities<" + containedType + ">";
                                    break;
                                default:
                                    fieldSetting.CsType = csTypes[fieldSetting.CmsType];
                                    fieldSetting.Method = "GetValue<" + fieldSetting.CsType + ">";
                                    break;
                            }
                        }
                    }
                    else
                    {
                        csTypes.Add(fieldSetting.CmsType, "string");

                        // Add the unknown type to the configuration xml file, so it's easier to create the correct conversion.
                        this._settings.AddDefaultCmsTypeConversion(this._name, fieldSetting.CmsType);
                    }
                    if (methods.ContainsKey(fieldSetting.CmsType) && !string.IsNullOrEmpty(methods[fieldSetting.CmsType]))
                        fieldSetting.Method = methods[fieldSetting.CmsType];
                }
        }

        private void OverrideByCustomSettings(XmlElement root)
        {
            foreach (XmlElement template in root.SelectNodes("CustomSettings/Template"))
            {
                string path = template.GetAttribute("path");
                if (this._templateSettings.ContainsKey(path))
                {
                    TemplateSetting setting = this._templateSettings[path];
                    foreach (XmlElement fieldSetting in template.ChildNodes)
                    {
                        string name = fieldSetting.GetAttribute("name");
                        if (setting.FieldSettings.ContainsKey(name))
                        {
                            var f = setting.FieldSettings[name];
                            bool skip = fieldSetting.GetAttribute("skip") == "true";
                            if (skip)
                                setting.FieldSettings.Remove(name);
                            else
                            {
                                f.CsType = fieldSetting.GetAttribute("cstype");
                                string method = fieldSetting.GetAttribute("method");
                                if (string.IsNullOrEmpty(method))
                                    f.Method = "GetValue<" + f.CsType + ">";
                                else
                                    f.Method = method;
                            }
                        }
                    }
                }
            }
        }

        private void RemoveSystemFields()
        {
            foreach (var template in this._templateSettings.Values)
            {
                string[] removedKeys = template.FieldSettings.Keys.Where(k => k.StartsWith("__")).ToArray();
                foreach (string key in removedKeys)
                    template.FieldSettings.Remove(key);
            }
        }

        private void TrimTemplatesByIncludePaths(XmlElement root)
        {
            string[] paths = root.SelectNodes("IncludePaths/Path").Cast<XmlElement>().Select(e => e.InnerText).ToArray();
            string[] keys = this._templateSettings.Keys.ToArray();
            foreach (string key in keys)
            {
                if (!paths.Where(p => key.IsWildcardMatch(p)).Any())
                    this._templateSettings.Remove(key);
            }
        }

        private void TrimTemplatesByExcludePaths(XmlElement root)
        {
            string[] paths = root.SelectNodes("ExcludePaths/Path").Cast<XmlElement>().Select(e => e.InnerText).ToArray();
            string[] keys = this._templateSettings.Keys.ToArray();
            foreach (string key in keys)
            {
                if (paths.Where(p => key.IsWildcardMatch(p)).Any())
                    this._templateSettings.Remove(key);
            }
        }

        #endregion Methods

        #region Nested Types

        public class FieldSetting
        {
            #region Constructors

            public FieldSetting(TemplateField field)
            {
                this.Name = field.Name;
                this.CmsType = field.Type;
                this.AccessModifier = "public";
                this.CsType = "string";
                this.Method = "GetValue<" + CsType + ">";
                this.Section = field.Section;

                if (field.CodeGenerationSettings != null)
                {
                    this.ContainedType = field.CodeGenerationSettings.ContainedType;
                    this.Skip = field.CodeGenerationSettings.Skip;
                }
            }

            #endregion Constructors

            #region Properties

            public string AccessModifier
            {
                get; set;
            }

            public string CmsType
            {
                get; set;
            }

            public string ContainedType
            {
                get; private set;
            }

            public string CsType
            {
                get; set;
            }

            public string Method
            {
                get; set;
            }

            public string Name
            {
                get; set;
            }

            public string PropertyName
            {
                get; set;
            }

            public string Section
            {
                get; private set;
            }

            public bool Skip
            {
                get; private set;
            }

            #endregion Properties

            public string TypeTable { get; set; }
        }

        public class TemplateSetting
        {
            #region Fields

            private readonly Dictionary<string, FieldSetting> _fieldSettings;

            #endregion Fields

            #region Constructors

            public TemplateSetting(Template template)
            {
                Name = template.Name;
                Path = template.Path;
                Id = template.Id;
                BaseClass = "Entity";
                AccessModifier = "public";
                _fieldSettings = new Dictionary<string, FieldSetting>();
                ClassName = GetCsName(Name);
                foreach (var field in template.AllFields)
                {
                    if (!_fieldSettings.ContainsKey(field.Name))
                    {
                        FieldSetting fieldSetting = new FieldSetting(field);
                        if (!fieldSetting.Skip)
                            _fieldSettings.Add(fieldSetting.Name, fieldSetting);
                    }
                }
            }

            #endregion Constructors

            #region Properties

            public string AccessModifier
            {
                get; set;
            }

            public string BaseClass
            {
                get; set;
            }

            public string ClassName
            {
                get; private set;
            }

            public Dictionary<string, FieldSetting> FieldSettings
            {
                get { return _fieldSettings; }
            }

            public Id Id
            {
                get; private set;
            }

            public string Name
            {
                get; private set;
            }

            public string NameSpace
            {
                get; set;
            }

            public string Path
            {
                get; private set;
            }

            #endregion Properties

            #region Methods

            public FieldSetting GetFieldSetting(string fieldName)
            {
                return _fieldSettings[fieldName];
            }

            #endregion Methods
        }

        #endregion Nested Types
    }

    public class CodeGenerationSettings
    {
        #region Fields

        private Dictionary<string, CodeGenerationConfiguration> _Configurations;
        private Dictionary<string, string> _CsTypeConversions;
        private string _DefaultCsTypeConversion;

        #endregion Fields

        #region Constructors

        private CodeGenerationSettings()
        {
            XmlDocument doc = GetXml();

            _Configurations = new Dictionary<string, CodeGenerationConfiguration>();
            foreach (XmlElement config in doc.SelectNodes("CodeGeneration/Configuration"))
            {
                string name = config.GetAttribute("name");
                CodeGenerationConfiguration configuration = new CodeGenerationConfiguration(this, name, config);
                _Configurations.Add(name, configuration);
            }

            _CsTypeConversions = new Dictionary<string, string>();
            XmlElement csConversion = (XmlElement)doc.SelectSingleNode("CodeGeneration/CsTypeConversions");
            _DefaultCsTypeConversion = csConversion.GetAttribute("defaultValue");
            foreach (XmlElement type in csConversion.ChildNodes)
            {
                string typeName = type.GetAttribute("name");
                if (_CsTypeConversions.ContainsKey(typeName))
                    continue;
                string convertsTo = type.GetAttribute("convertsTo");
                if (convertsTo == "?")
                    _CsTypeConversions.Add(typeName, _DefaultCsTypeConversion);
                else
                    _CsTypeConversions.Add(typeName, convertsTo);
            }
        }

        #endregion Constructors

        #region Properties

        public static CodeGenerationSettings Instance
        {
            get { return Cache.Get("CodeGenerationSettings", CacheScope.Request, () => new CodeGenerationSettings()); }
        }

        public string[] Configurations
        {
            get { return _Configurations.Keys.ToArray(); }
        }

        #endregion Properties

        #region Indexers

        public CodeGenerationConfiguration this[string key]
        {
            get { return _Configurations[key]; }
        }

        #endregion Indexers

        #region Methods

        public CodeGenerationConfiguration GetConfiguration(string name)
        {
            return _Configurations[name];
        }

        public string GetCsTypeConversion<T>()
        {
            string typeName = typeof (T).Name;
            if (_CsTypeConversions.ContainsKey(typeName))
                return _CsTypeConversions[typeName];

            AddDefaultCsTypeConversion(typeName);
            _CsTypeConversions.Add(typeName, _DefaultCsTypeConversion);
            return _DefaultCsTypeConversion;
        }

        internal void AddDefaultCmsTypeConversion(string configuration, string cmsType)
        {
            XmlDocument doc = GetXml();
            XmlElement conversions = (XmlElement)doc.SelectSingleNode("CodeGeneration/Configuration[@name='" + configuration + "']/DefaultSettings");
            XmlElement typeElement = doc.CreateElement("FieldSetting");
            conversions.AppendChild(typeElement);
            typeElement.SetAttribute("cmstype", cmsType);
            typeElement.SetAttribute("cstype", "string");
            doc.Save(GetFilename());
        }

        private void AddDefaultCsTypeConversion(string typeName)
        {
            XmlDocument doc = GetXml();
            XmlElement conversions = (XmlElement) doc.SelectSingleNode("CodeGeneration/CsTypeConversions");
            XmlElement typeElement = doc.CreateElement("type");
            conversions.AppendChild(typeElement);
            typeElement.SetAttribute("name", typeName);
            typeElement.SetAttribute("convertsTo", "?");
            doc.Save(GetFilename());
        }

        private string GetFilename()
        {
            return HttpContext.Current.Server.MapPath("~/App_Data/CodeGeneration.xml");
        }

        private XmlDocument GetXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(GetFilename());
            return doc;
        }

        #endregion Methods
    }
}