using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class ProductLine:EntryInfo
    {
        public int ProductLineID { get; set; }
        
        [Display(Name="Product Name")]
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
        [ForeignKey("Job")]
        public int JobID { get; set; }
        public virtual Job Job { get; set; }
        public virtual ICollection<ProductDetail> Details { get; set; }
        public int? ServiceProductID { get; set; }
        public int? WarrantyProductID { get; set; }
        public ProductLine()
        {
            Details = new HashSet<ProductDetail>();
            
        }

        public string DisplayText
        {
            get
            {
                string properties = "";
                foreach (var property in Details)
                {
                    if ((property.Value ?? "") != "")
                    {
                        properties = properties + " " + property.Value;
                    }

                }
                return (Product ?? new Product()).ProductName + "# " + properties;
            }
        }
        public string Property
        {
            get
            {
                string properties = "";
                foreach (var property in Details)
                {
                    if ((property.Value ?? "") != "")
                    {
                        properties = properties + " " + property.Value;
                    }

                }
                return properties;
            }
        }
        public string SearchText
        {
            get
            {
                return (Product ?? new Product()).ProductName + " "
                    +Property;
            }
        }

    }
}