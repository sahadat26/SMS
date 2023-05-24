using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class UserInfo
    {
        public EModule Module { get; set; }
        public EBusinessUnit BusinessUnit { get; set; }
        public User User { get; set; }
        public string Host { get; set; }

        public static UserInfo GetUserInfo(string CookieVal)
        {
            var CookieArr = CookieVal.Split('|');
            if(CookieArr.Length!=3)
            {
                throw new Exception("Please Log In Again");
            }
            UserInfo UI = new UserInfo();
            UI.Module = (EModule)Enum.Parse(typeof(EModule), CookieArr[0]);
            UI.BusinessUnit = (EBusinessUnit)Enum.Parse(typeof(EBusinessUnit), CookieArr[1]);
            UI.User = User.getUser(CookieVal);
            

            return UI;
        }

        public UserInfo()
        {
            
            User = new User();
        }
    }
}