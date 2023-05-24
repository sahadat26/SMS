using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class SC_ASETarget:EntryInfo
    {
        public int ID { get; set; }
        public EBusinessUnit BU { get; set; }
        [Display(Name="ASE Name")]
        [ForeignKey("ASE")]
        public int ASEUID { get; set; }
        public virtual User ASE { get; set; }
        [Display(Name = "Month Name")]
        [Required]
       // public string MonthYearName { get; set; }
        public EMonth MonthYearName { get; set; }

        [Display(Name = "Year Name")]
        [Required]
        public int? Year { get; set; }


        public ETargetType TargetType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Display(Name = "Target Sales Amount")]
        public decimal TargetAmount { get; set; }

        public decimal AchievedPerc { get; set; }

        [Display(Name = "Target Sales Collection Amount")]
        public decimal AchievedAmount { get; set; }
        [Display(Name = "New Contract")]
        [Range(0, 100, ErrorMessage = "Only Solid Number Allowed")]
        public int NewContract { get; set; }
        [Display(Name = "Renew Contract")]
        [Range(0,100,ErrorMessage="Only Solid Number Allowed")]
        public int RenewContract { get; set; }


        public string SearchText
        {
            get
            {
                return Enum.GetName(typeof(EMonth), MonthYearName) + " " + (ASE ?? new User()).DisplayText + " " + Enum.GetName(typeof(ETargetType), TargetType);
            }
        }
    }
}