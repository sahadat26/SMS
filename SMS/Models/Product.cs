using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class Product:EntryInfo
    {
        public int ProductID { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Display(Name="Product Category")]
        [ForeignKey("ProductCategory")]
        public int ProductCategoryID { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public EBusinessUnit BusinessUnit { get; set; }
        [Required]
        [StringLength(10)]
        public string Prefix { get; set; }
        [StringLength(100)]
        public string ImageURL { get; set; }
        public virtual ICollection<ProductProperty> Properties { get; set; }
        
        public Product()
        {
            Properties = new HashSet<ProductProperty>();
        }

        public string Attribute
        {
            get
            {
                if (Properties.Count > 0)
                {
                    string attr = "";
                    foreach(var p in Properties)
                    {
                        attr = attr + " " + p.Property.Name;
                    }
                }

                return "";
            }
        }

    }
}