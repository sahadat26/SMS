using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Models
{
    public class EmailText
    {
        [Key]
        [Display(Name="Trigger On Event")]
        public EActions Action { get; set; }
        [Required]
        public string Subject { get; set; }
        [AllowHtml]
        [Required]
        public string Body { get; set; }
        public string Message { get; set; }
    }
}