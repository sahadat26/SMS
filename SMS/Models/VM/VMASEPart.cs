using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class VMASEPart
    {
        [Display(Name="Invoice No")]
        public string InvoiceNo { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Spare Amount")]
        public decimal SpareAmount { get; set; }
        [Display(Name = "ASE Part Amount")]
        public decimal ASEPartAmount { get; set; }
        [Range(1,100,ErrorMessage="Percent value should 1 to 100")]
        [Display(Name = "ASE Part Percent")]
        public decimal ASEPartPerc { get; set; }
    }
}