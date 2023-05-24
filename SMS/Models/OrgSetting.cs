using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class OrgSetting
    {
        [Key]
        public string ProfitCenter { get; set; }
        public EBusinessUnit BU { get; set; }
        public string SalesOrg { get; set; }
        public string Division { get; set; }
        public string ProductSalesOrg { get; set; }
    }
}