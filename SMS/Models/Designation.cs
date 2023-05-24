using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class Designation:EntryInfo
    {
        public int DesignationID { get; set; }
        public EBusinessUnit BU { get; set; }
        public string JobGrade { get; set; }
        [Required]
        [Display(Name="Designation Name")]
        public string Name { get; set; }
        [Range(1,9999,ErrorMessage="Invalid Amount")]
        [Display(Name = "Working Hour Rate")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WorkingHourRate { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Holiday Hour Rate")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal HolidayHourRate { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "OT Hour Rate")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal OTHourRate { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Food Allowance")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal FoodAllowance { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Daily Tour Allowance")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal DailyAllowance { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Daily Tour Allowance with Company Vehicle")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal DailyAllowanceCV { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Metropolitan City Accomodation")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal MCAccomodation { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Other City Accomodation")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal OCAccomodation { get; set; }
        public string SearchText
        {
            get
            {
                return DesignationID + " " + Name;
            }
        }
    }
}