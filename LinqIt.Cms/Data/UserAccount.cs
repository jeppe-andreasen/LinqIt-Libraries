namespace LinqIt.Cms.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public abstract class UserAccount
    {
        #region Properties

        public abstract string Email
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public abstract bool HasRole(string roleName);

        public abstract void AddRole(string roleName);

        public abstract void RemoveRole(string roleName);

        public abstract string ResetPassword();

        public abstract bool ChangePassword(string oldPassword, string newPassword);

        public abstract void AssignRoles(Dictionary<string, bool> roles, bool removeUnselectedRoles);

        public abstract bool IsAnonymous { get; }

        public abstract string ProfileType { get; set; }

        public abstract bool IsApproved { get; set; }

        public abstract string this[string key] { get; set; }
        
        #endregion Properties

        public abstract void Save();

        public abstract void LogOff();
    }
}
