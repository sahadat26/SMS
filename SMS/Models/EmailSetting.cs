using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Models
{
    public class EmailSetting
    {
        [Key]
        public string Name { get; set; }
        [Required]
        public string SMTPServer { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string FromEmail { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int Port { get; set; }
        [AllowHtml]
        public string Signature { get; set; }

        public string EmailAddress
        {
            get
            {
                return DisplayName + "<" + FromEmail + ">";
            }
        }
    }
}