using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using SMS.Models;
using System.Security.Cryptography;
using System.Text;

namespace SMS
{
    public class AuthenticationMembershipProvider:MembershipProvider
    {
        //test
        private AuthenticationRepository repository;

        public AuthenticationMembershipProvider()
        {
            this.repository = new AuthenticationRepository();
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

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword) || string.IsNullOrEmpty(newPassword.Trim()))
                return false;
            
            User user = repository.GetUser(username);
            string hash = CommonMethod.GetMD5(newPassword.Trim());
            
            user.Password = hash;
            user.ConfirmPassword = hash;
            repository.Save();

            return true;
        }

        

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        //public void CreateUser(RegisterModel Model)
        //{
        //    this.repository.CreateUser(Model);
        //}

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 4; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(password.Trim()) || string.IsNullOrEmpty(username.Trim()))
                return false;

            string hash = CommonMethod.GetMD5(password.Trim());
 
            return this.repository.GetAllUsers().Any(user => (user.UserName==username.Trim()) && (user.Password == hash)&&(user.Exist==true));
        }

        public bool AuthenticateUser(LogOnModel model)
        {
            bool flag = false;
            if (string.IsNullOrEmpty(model.Password.Trim()) || string.IsNullOrEmpty(model.UserName.Trim()))
                return flag;

            string hash = CommonMethod.GetMD5(model.Password.Trim());

            
            var u = this.repository.GetAllUsers().Where(user => (user.UserName == model.UserName.Trim()) 
                && (user.Password == hash)
                && (user.Exist == true)).FirstOrDefault();
            if(u==null)
            {
                flag = false;
            }
            else if(u.BusinessUnit==EBusinessUnit.ALL)
            {
                flag = true;
            }
            else if (u.BusinessUnit == model.BusinessUnit)
            {
                flag = true;
            }
            return flag;
        }
    
    }

}
