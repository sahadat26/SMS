using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class ServiceProductDetail:EntryInfo
    {
        public int ServiceProductDetailID { get; set; }
        [ForeignKey("ServiceProduct")]
        public int ServiceProductID { get; set; }
        public virtual ServiceProduct ServiceProduct { get; set; }
        [ForeignKey("ProductProperty")]
        public int ProductPropertyID { get; set; }
        public virtual ProductProperty ProductProperty { get; set; }
        [StringLength(100)]
        public string Value { get; set; }
    }
}