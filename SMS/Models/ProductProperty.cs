using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class ProductProperty
    {
        public int ProductPropertyID { get; set; }
        public int Sl { get; set; }
        [ForeignKey("Product")]
        [Display(Name="Product")]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
        [ForeignKey("Property")]
        public int PopertyID { get; set; }
        public virtual Property Property { get; set; }
        public bool IsRequired { get; set; }
        public bool IsUnique { get; set; }
        [NotMapped]
        public List<string> PropertyValue { get; set; }
    }
}