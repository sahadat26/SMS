using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class WarrantyContract:EntryInfo
    {
        public int WarrantyContractID { get; set; }
        public EBusinessUnit BusinessUnit { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM, yyyy}")]
        public DateTime PurchaseDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM, yyyy}")]
        public DateTime InstallDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM, yyyy}")]
        public DateTime WarrantyStart { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM, yyyy}")]
        public DateTime WarrantyEnd { get; set; }
        [Display(Name = "Customer")]
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        [Display(Name = "Contact")]
        [ForeignKey("Contact")]
        public int? ContactID { get; set; }
        public virtual Contact Contact { get; set; }
        
        [Display(Name="Warranty Expired on Running Hour")]
        public double ExpiredRunningHour { get; set; }
        
        [StringLength(100)]
        public string AttachmentURL { get; set; }
        public EStatus Status { get; set; }

        public virtual ICollection<WarrantyProduct> Details { get; set; }

        public WarrantyContract()
        {
            Details = new HashSet<WarrantyProduct>();
        }

        public string DisplayText
        {
            get
            {
                
                return (Customer ?? new Customer()).Name + "# " + Products;
            }
        }
        public decimal TotalServiceAmount
        {
            get
            {
                if (Details.Count == 0)
                    return 0;
                return Details.Sum(d => d.ServiceAmount);
            }
        }
        public string Products
        {
            get
            {
                string products = "";
                foreach (var product in Details)
                {


                    products = products + " " + product.Product.ProductName;

                }
                return products;
            }
        }

        public string SearchText
        {
            get
            {
                return WarrantyContractID+" "+
                    Status
                    + " " + (Customer ?? new Customer()).Name + " " + Products;
            }
        }
    }
}