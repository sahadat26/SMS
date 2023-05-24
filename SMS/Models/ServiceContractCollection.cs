using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class ServiceContractCollection:EntryInfo
    {
        public int ServiceContractCollectionID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM, yyyy}")]
        [Display(Name="Collection Date")]
        public DateTime CollectionDate { get; set; }
        [Display(Name="Collected Amount")]
        public decimal CollectedAmount { get; set; }
        [Display(Name="Service Product")]
        [ForeignKey("ServiceProduct")]
        public int ServiceProductID { get; set; }
        public virtual ServiceProduct ServiceProduct { get; set; }
    }
}