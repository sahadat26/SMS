using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class WarrantyProductDetail:EntryInfo
    {
        public int WarrantyProductDetailID { get; set; }
        [ForeignKey("WarrantyProduct")]
        public int WarrantyProductID { get; set; }
        public virtual WarrantyProduct WarrantyProduct { get; set; }
        [ForeignKey("ProductProperty")]
        public int ProductPropertyID { get; set; }
        public virtual ProductProperty ProductProperty { get; set; }
        [StringLength(100)]
        public string Value { get; set; }
    }
}