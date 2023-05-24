using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using SMS.Models;
using System.Web.Mvc;

namespace SMS
{
    public class AuthenticationRoleProvider:RoleProvider
    {
        /// <summary>
        /// test
        /// </summary>
        private AuthenticationRepository repository;

        public AuthenticationRoleProvider()
        {
            this.repository = new AuthenticationRepository();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
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

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string userName)
        {
            if(userName.Split('|').Count()>1)
            {
                var UI = UserInfo.GetUserInfo(userName);
                userName = UI.User.UserName;
            }
            IQueryable<Role> role = this.repository.GetRolesForUser(userName);
            //if (!this.repository.RoleExists(role))
            //    return new string[] { string.Empty };
            string[] roles=new string[role.Count()];
            int i=0;
            foreach (Role _r in role)
            {
                roles[i++] = _r.Name;
            }
            return roles;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string userName, string roleName)
        {
            if (userName.Split('|').Count() > 1)
            {
                var UI = UserInfo.GetUserInfo(userName);
                userName = UI.User.UserName;
                
            }
            User user = repository.GetUser(userName);
            Role role = repository.GetRole(roleName);

            if (!repository.UserExists(user))
                return false;
            if (!repository.RoleExists(role))
                return false;

            return user.Roles.Any(i=>i.RoleID==role.RoleID);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}