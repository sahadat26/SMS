using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class User
    {
        
        public int UserID { get; set; }
        [Display(Name="User Name")]
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        [StringLength(100)]
        [Required]
        public string FullName { get; set; }
        [ForeignKey("Designation")]
        [Display(Name="Designation")]
        public int DesignationID { get; set; }
        public virtual Designation Designation { get; set; }
        [Range(1,4,ErrorMessage="Invalid Selection")]
        [Display(Name="Business Unit")]
        public EBusinessUnit BusinessUnit { get; set; }
        [StringLength(100)]
        [Required]
        public string Department { get; set; }
        public bool Exist { get; set; }
        [Display(Name="Supervisor")]
        public int? SupervisorID { get; set; }

        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
       
        [DisplayFormat(DataFormatString="{0:dd MMM, yyyy}")]
        public DateTime CreateDate { get; set; }
        [StringLength(100)]
        
        //[EmailAddress]
        public string Email { get; set; }
        public bool IsASE { get; set; }
        public string ProfitCenter { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<SC_ASEApprovedData> SC_ASEApprovedData { get; set; }

        [Display(Name="Basic Salary")]
        public decimal? BasicSalary { get; set; }
         [Display(Name = "Conveyance Allowance")]
        public decimal? ConveyanceAllowance { get; set; }
        [Display(Name = "Food Allowance")]
        public decimal? FoodAllowance { get; set; }
        [Display(Name = "Mobile Allowance")]
        public decimal? MobileAllowance { get; set; }
        [Display(Name = "Additional Allowance")]
        public decimal? AdditionalAllowance { get; set; }

        [Display(Name = "Deduction Allowance")]
        public decimal? DeductionAllowance { get; set; }

        [Display(Name = "Wing")]
        public string Wing { get; set; }

        [Display(Name="User Type")]
        public EUserType? userType { get; set; }
        public string SAPCode
        {
            get
            {
                return "EP" + UserName;
            }
        }

        public User()
        {
            Roles = new HashSet<Role>();
            SC_ASEApprovedData = new HashSet<SC_ASEApprovedData>();
            
        }

        public User Supervisor
        {
            get
            {
                if (SupervisorID == null)
                    return new User();
                SMSContext db = new SMSContext();
                return db.Users.Find(SupervisorID);
            }
        }
        public static User getUser(string cookieName)
        {
            SMSContext db = new SMSContext();
            var ust = cookieName.Split();
            var username = cookieName.Split('|')[2];
            User _User = db.Users.Where(u=>u.UserName==username).FirstOrDefault();
            return _User;
        }

        public static User getSupervisor(int UID)
        {
            SMSContext db = new SMSContext();
            
            User _User = db.Users.Where(u => u.UserID==UID).FirstOrDefault();
            return _User;
        }

        public static SelectList GetUserSelectList()
        {
            SMSContext db = new SMSContext();
            var list=db.Users.Where(u=>u.UserName!="admin");
            return new SelectList(list,"UserID","FullName");
        }

        public string DisplayText
        {
            get
            {
                return UserName + "-" + FullName;
            }
        }

        public string SearchText
        {
            get
            {
                return UserName + " " + FullName + " " + BusinessUnit + " " + Department + " " + Email + " " + (Designation ?? new Designation()).JobGrade+ " " + (Designation ?? new Designation()).Name;
            }
        }


    }
}