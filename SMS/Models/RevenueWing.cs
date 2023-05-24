using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class RevenueWing:EntryInfo
    {
        public int RevenueWingID { get; set; }
        public EBusinessUnit BU { get; set; }
        [Required]
        public string Name { get; set; }
    }
}