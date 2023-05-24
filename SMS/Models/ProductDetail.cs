using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class ProductDetail:EntryInfo
    {
        public int ProductDetailID { get; set; }
        [ForeignKey("ProductLine")]
        public int ProductLineID { get; set; }
        public virtual ProductLine ProductLine { get; set; }
        [ForeignKey("ProductProperty")]
        public int ProductPropertyID { get; set; }
        public virtual ProductProperty ProductProperty { get; set; }
        [StringLength(100)]
        public string Value { get; set; }
    }
}