using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class SC_ASEContribution:EntryInfo
    {
        [Key]
        public int ID { get; set; }
        public int SL { get; set; }
        public EBusinessUnit BU { get; set; }
        [Display(Name="ASE Name")]
        [ForeignKey("ASE")]
        public int ASEID { get; set; }
        public virtual User ASE { get; set; }
        [ForeignKey("Sale")]
        public string Invoice_Doc { get; set; }
        public virtual SC_Sales Sale { get; set; }
        [Range(1,100,ErrorMessage="Between 1 to 100")]
        [NotMapped]
        [Display(Name="In %")]
        public decimal ContrPerc { get; set; }
        [Display(Name = "In Amount")]
        public decimal ContrAmount { get; set; }
        [NotMapped]
        public decimal SpareAmount { get; set; }
 
        public decimal DisConrPerc
        {
            get
            {
                int perc = 0;
                if(SpareAmount>0)
                {
                    return (100 * ContrAmount) / SpareAmount;
                }

                return perc;
            }
        }
        public string SearchText
        {
            get
            {
                return Invoice_Doc + " " + (Sale ?? new SC_Sales()).Customer_ID + " " + (Sale ?? new SC_Sales()).Customer_Name + " " + (Sale ?? new SC_Sales()).SM_ID + " " + (Sale ?? new SC_Sales()).SM_Name + " " + (Sale ?? new SC_Sales()).SP_ID + " " + (Sale ?? new SC_Sales()).SP_Name;
            }

        }

        
    }
}