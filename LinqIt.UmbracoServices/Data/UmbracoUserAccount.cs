using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Cms.Data;
using umbraco.BusinessLogic;

namespace LinqIt.UmbracoServices.Data
{
    public class UmbracoUserAccount : UserAccount
    {
        private readonly User _user;

        internal UmbracoUserAccount(User user)
        {
            _user = user;
        }

        public override string Email
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { return _user.Name; }
        }

        public override bool HasRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword()
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override void AssignRoles(Dictionary<string, bool> roles, bool removeUnselectedRoles)
        {
            throw new NotImplementedException();
        }

        public override bool IsAnonymous
        {
            get { throw new NotImplementedException(); }
        }

        public override string ProfileType
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsApproved
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override string this[string key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }

        public override void LogOff()
        {
            throw new NotImplementedException();
        }
    }
}
