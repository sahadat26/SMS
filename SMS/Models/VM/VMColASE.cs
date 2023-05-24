using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class VMColASE
    {
        public int ID { get; set; }
        [Display(Name="Payment Doc")]
        public string DocNo { get; set; }
        [Display(Name = "Fiscal Year")]
        public int FY { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Collected Amount")]
        public decimal Amount { get; set; }
        [Display(Name="Collector ASE")]
        public int ASEUID { get;set;}
        
    }
}