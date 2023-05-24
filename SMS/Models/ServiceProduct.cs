using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class ServiceProduct:EntryInfo
    {
        public int ServiceProductID { get; set; }
        
        [Display(Name="Product Name")]
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
        [Display(Name = "Service Contract")]
        [ForeignKey("ServiceContract")]
        public int ServiceContractID { get; set; }
        public decimal ContractAmount { get; set; }
        public virtual ServiceContract ServiceContract { get; set; }
        public virtual ICollection<ServiceProductDetail> Details { get; set; }
        public virtual ICollection<ServiceContractCollection> Collections { get; set; }
        public ServiceProduct()
        {
            Details = new HashSet<ServiceProductDetail>();
            Collections = new HashSet<ServiceContractCollection>();
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

        public decimal CollectedAmount(DateTime? Start,DateTime? End)
        {
            
            if (Collections.Count > 0)
            {
                if((Start==null)||(End==null))
                {
                    return Collections.Sum(c => c.CollectedAmount);
                }
                else
                {
                    return Collections.Where(c=>c.CollectionDate>=Start&&c.CollectionDate<=End)
                        .Sum(c => c.CollectedAmount);
                }
            }
                
            return 0;
            
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