namespace LinqIt.Cms.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SystemLink : Entity
    {
        #region Methods

        public T GetLinkedEntity<T>() where T : Entity, new()
        {
            return GetEntity<T>("Link");
        }

        #endregion Methods
    }
}