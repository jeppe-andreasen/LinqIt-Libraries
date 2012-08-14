namespace LinqIt.Cms.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class EntityTypeTable
    {
        #region Fields

        private Dictionary<string, Type> _types;

        #endregion Fields

        #region Constructors

        public EntityTypeTable()
        {
            _types = new Dictionary<string, Type>();
        }

        #endregion Constructors

        #region Methods

        public void AddType<T>()
            where T : Entity, new()
        {
            T item = new T();
            _types.Add(item.TemplatePath, typeof(T));
        }

        public void AddType(string templatePath, Type type)
        {
            _types.Add(templatePath, type);
        }

        public Type GetType(string templatePath)
        {
            if (_types.ContainsKey(templatePath))
                return _types[templatePath];
            return typeof (Entity);
        }

        #endregion Methods
    }
}