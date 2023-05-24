using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class WarrantyProduct:EntryInfo
    {
        public int WarrantyProductID { get; set; }
        
        [Display(Name="Product Name")]
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
        [Display(Name = "Warranty Contract")]
        [ForeignKey("WarrantyContract")]
        public int WarrantyContractID { get; set; }
        public virtual WarrantyContract WarrantyContract { get; set; }
        public virtual ICollection<WarrantyProductDetail> Details { get; set; }
        public decimal ServiceAmount { get; set; }
        public WarrantyProduct()
        {
            Details = new HashSet<WarrantyProductDetail>();
            
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