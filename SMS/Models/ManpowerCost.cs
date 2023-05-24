using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class ManpowerCost:EntryInfo
    {
        public int ManpowerCostID { get; set; }
        [Display(Name="Job Name")]
        [ForeignKey("Job")]
        public int JobID { get; set; }
        public virtual Job Job { get; set; }
        [Display(Name = "Service People")]
        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }
        public virtual User Employee { get; set; }
        [StringLength(200)]
        [Required]
        public string WorkDescription { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM, yy HH:mm}")]
        public DateTime StartTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM, yy HH:mm}")]
        public DateTime EndTime { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Working Hour")]
        public decimal WorkingHour { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Holiday Hour")]
        public decimal HolidayHour { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "OT Hour")]
        public decimal OTHour { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Travel Hour")]
        public decimal TravelHour { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Number of Meal")]
        public int NumberOfMeal { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Transport Cost")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal TransportCost { get; set; }
        [Display(Name = "Other Cost")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal OtherCost { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Tour Day")]
        public int NumberOfDay { get; set; }
        [Display(Name="Accomodation Type")]
        public EAccomodationType AccomodationType { get; set; }
        [Display(Name = "Daily Allowance Type")]
        public EDailyAllowanceType DailyAllowanceType { get; set; }
        #region Rate From Designation
        [Range(1, 9999, ErrorMessage = "Invalid Amount")]
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
        [Display(Name = "Daily Allowance")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal DailyAllowance { get; set; }
        [Range(0, 9999, ErrorMessage = "Invalid Amount")]
        [Display(Name = "Accomodation")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Accomodation { get; set; }
        #endregion

        #region Derive
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal TotalExpense
        {
            get
            {
                var HourlyCost = (WorkingHour * WorkingHourRate) + (HolidayHour * HolidayHourRate) + (OTHour * OTHourRate);
                
                return HourlyCost+(FoodAllowance*NumberOfMeal)+TransportCost+TotalAccomodation+TotalDailyAllowance+OtherCost;
            }
        }
        [DisplayFormat(DataFormatString="{0:N2}")]
        public decimal ManHourCost
        {
            get
            {
                var HourlyCost = (WorkingHour * WorkingHourRate) + (HolidayHour * HolidayHourRate) + (OTHour * OTHourRate);

                return HourlyCost;
            }
        }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal FoodCost
        {
            get
            {
                return NumberOfMeal * FoodAllowance;
            }
        }
        public decimal TotalDailyAllowance
        {
            get
            {
                return NumberOfDay * DailyAllowance;
            }
        }
        public decimal TotalAccomodation
        {
            get
            {
                return NumberOfDay * Accomodation;
            }
        }
        public string DateRange
        {
            get
            {
                return StartTime.ToString("dd-MMM-yy HH:mm") + " to " + EndTime.ToString("dd-MMM-yy HH:mm");
            }
        }
        #endregion
    }
}