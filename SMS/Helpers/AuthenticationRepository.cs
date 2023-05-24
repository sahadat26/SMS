using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMS.Models;
using System.Web.Security;
using System.Web.Mvc;
namespace SMS
{
    public class AuthenticationRepository
    {
        #region Variables
        //test
        private SMSContext entities = new SMSContext();
 
        private const string MissingRole = "Role does not exist";
        private const string MissingUser = "User does not exist";
        private const string TooManyUser = "User already exists";
        private const string TooManyRole = "Role already exists";
        private const string AssignedRole = "Cannot delete a role with assigned users";
 
        #endregion
 
        #region Properties
 
        public int NumberOfUsers
        {
            get
            {
                return this.entities.Users.Count();
            }
        }
 
        public int NumberOfRoles
        {
            get
            {
                return this.entities.Roles.Count();
            }
        }
 
        #endregion
 
        #region Constructors
 
        public AuthenticationRepository()
        {
            this.entities = new SMSContext();
        }
 
        #endregion
 
        #region Query Methods
 
        public IQueryable<User> GetAllUsers()
        {
            return from user in entities.Users
                   orderby user.UserName
                   select user;
        }
 
        public User GetUser(int id)
        {
            return entities.Users.SingleOrDefault(user => user.UserID == id);
        }
 
        public User GetUser(string userName)
        {
            if (userName.Split('|').Count() > 1)
            {
                var UI = UserInfo.GetUserInfo(userName);
                userName = UI.User.UserName;
            }
            return entities.Users.Where(user=>user.UserName==userName).SingleOrDefault();
        }
 
        public IQueryable<User> GetUsersForRole(string roleName)
        {
            return GetUsersForRole(GetRole(roleName));
        }
 
        public IQueryable<User> GetUsersForRole(int id)
        {
            return GetUsersForRole(GetRole(id));
        }
 
        public IQueryable<User> GetUsersForRole(Role role)
        {
            if (!RoleExists(role))
                throw new ArgumentException(MissingRole);

            return from row in entities.Users
                   where row.Roles.Contains(role)
                   select row;
        }
 
        public IQueryable<Role> GetAllRoles()
        {
            return from role in entities.Roles
                   orderby role.Name
                   select role;
        }
 
        public Role GetRole(int id)
        {
            return entities.Roles.SingleOrDefault(role => role.RoleID == id);
        }
 
        public Role GetRole(string name)
        {
            return entities.Roles.SingleOrDefault(role => role.Name == name);
        }
 
        public IQueryable<Role> GetRolesForUser(string userName)
        {
            if (userName.Split('|').Count() > 1)
            {
                var UI = UserInfo.GetUserInfo(userName);
                userName = UI.User.UserName;
            }
            return GetRolesForUser(GetUser(userName));
        }
 
        public IQueryable<Role> GetRolesForUser(int id)
        {
            return GetRolesForUser(GetUser(id));
        }
 
        public IQueryable<Role> GetRolesForUser(User user)
        {
            if (!UserExists(user))
                throw new ArgumentException(MissingUser);
            SMSContext db = new SMSContext();
            var _User = db.Users.Include("Roles").Where(u=>u.UserName==user.UserName).FirstOrDefault();
            return _User.Roles.AsQueryable();
        }

 
        #endregion
 
        #region Insert/Delete
 
        private void AddUser(User user)
        {
            if (UserExists(user))
                throw new ArgumentException(TooManyUser);
 
            entities.Users.Add(user);
        }
 
        //public void CreateUser(RegisterModel Model)
        //{
            
 
        //    if (string.IsNullOrEmpty(Model.UserName.Trim()))
        //        throw new ArgumentException("The user name provided is invalid. Please check the value and try again.");
        //    if (string.IsNullOrEmpty(Model.FullName))
        //        throw new ArgumentException("The name provided is invalid. Please check the value and try again.");
        //    if (string.IsNullOrEmpty(Model.Password))
        //        throw new ArgumentException("The password provided is invalid. Please enter a valid password value.");
        //    if (this.entities.Users.Any(user => user.UserName == Model.UserName))
        //        throw new ArgumentException("Username already exists. Please enter a different user name.");
 
        //    User newUser = new User()
        //    {
        //        UserName = Model.UserName,
        //        FullName = Model.FullName,
        //        Password = FormsAuthentication.HashPasswordForStoringInConfigFile(Model.Password, "md5"),
                
        //        CreateDate=DateTime.Today,
        //        ActiveStatus=false
        //    };
 
        //    try
        //    {
        //        AddUser(newUser);
        //    }
        //    catch (ArgumentException ae)
        //    {
        //        throw ae;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ArgumentException("The authentication provider returned an error. Please verify your entry and try again. " +
        //            "If the problem persists, please contact your system administrator.");
        //    }
 
        //    // Immediately persist the user data
        //    Save();
        //}
 
        public void DeleteUser(User user)
        {
            if (!UserExists(user))
                throw new ArgumentException(MissingUser);
 
            entities.Users.Remove(user);
        }
 
        public void DeleteUser(string userName)
        {
            DeleteUser(GetUser(userName));
        }
 
        public void AddRole(Role role)
        {
            if (RoleExists(role))
                throw new ArgumentException(TooManyRole);
 
            entities.Roles.Add(role);
        }
 
        public void AddRole(string roleName)
        {
            Role role = new Role()
            {
                Name = roleName
            };
 
            AddRole(role);
        }
 
        public void DeleteRole(Role role)
        {
            if (!RoleExists(role))
                throw new ArgumentException(MissingRole);
 
            if (GetUsersForRole(role).Count() > 0)
                throw new ArgumentException(AssignedRole);
 
            entities.Roles.Remove(role);
        }
 
        public void DeleteRole(string roleName)
        {
            DeleteRole(GetRole(roleName));
        }
 
        #endregion
 
        #region Persistence
 
        public void Save()
        {
            entities.SaveChanges();
        }
 
        #endregion
 
        #region Helper Methods
 
        public bool UserExists(User user)
        {
            if (user == null)
                return false;
 
            return (entities.Users.SingleOrDefault(u => u.UserID == user.UserID || u.UserName == user.UserName) != null);
        }
 
        public bool RoleExists(Role role)
        {
            if (role == null)
                return false;
 
            return (entities.Roles.SingleOrDefault(r => r.RoleID == role.RoleID || r.Name == role.Name) != null);
        }
 
        #endregion
 
    }

 
}
