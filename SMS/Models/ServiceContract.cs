using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class ServiceContract:EntryInfo
    {
        public int ServiceContractID { get; set; }
        public EBusinessUnit BusinessUnit { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM, yyyy}")]
        public DateTime ContractStartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM, yyyy}")]
        public DateTime ContractEndDate { get; set; }
        [Display(Name = "Customer")]
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        [Display(Name = "Contact")]
        [ForeignKey("Contact")]
        public int? ContactID { get; set; }
        public virtual Contact Contact { get; set; }
        
        [Range(1,7,ErrorMessage="Invalid Billing Cycle")]
        public EDuration BillingCycle { get; set; }
        
        public int RevisionNo { get; set; }
        
        [StringLength(100)]
        public string AttachmentURL { get; set; }
        public EStatus Status { get; set; }

        public virtual ICollection<ServiceProduct> Details { get; set; }

        public ServiceContract()
        {
            Details = new HashSet<ServiceProduct>();
        }

        public decimal TotalContractAmount
        {
            get
            {
                if (Details.Count == 0)
                    return 0;
                return Details.Sum(d => d.ContractAmount);
            }
        }

        public decimal TotalCollectedAmount(DateTime? Start,DateTime? End)
        {
            if (Details.Count == 0)
                return 0;
            return Details.Sum(c => c.CollectedAmount(Start, End));
            
        }
        
        public string DisplayText
        {
            get
            {
                string products = "";
                foreach (var product in Details)
                {


                    products = products + " " + product.Product.ProductName;
                    
                }
                return (Customer ?? new Customer()).Name+" - "+products;
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
                return ServiceContractID+" "+
                    Status
                    + " " + (Customer ?? new Customer()).Name + " " + Products+" "+BillingCycle;
            }
        }
    }
}