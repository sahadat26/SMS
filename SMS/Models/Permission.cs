using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class Permission
    {
        [ForeignKey("Roles")]
        public int RoleID { get; set; }
        public virtual Role Roles { get; set; }
        public bool Status { get; set; }
    }
}