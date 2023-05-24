using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class Job:EntryInfo
    {
        #region Common
        public int JobID { get; set; }
        public EBusinessUnit BusinessUnit { get; set; }
        public EStatus Status { get; set; }
        public EState State { get; set; }
        public virtual ICollection<ProductLine> Products { get; set; }
        public virtual ICollection<ManpowerCost> ManpowerCosts { get; set; }
        public Job()
        {
            Products = new HashSet<ProductLine>();
            ManpowerCosts = new HashSet<ManpowerCost>();
        }
        #endregion
        #region Query
        [Range(1,3,ErrorMessage="Invalid Option for Query")]
        public EQueryFor QueryFor { get; set; }
        [Range(1, 3, ErrorMessage = "Invalid Query Type")]
        public EQueryType QueryType { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM, yyyy}")]
        public DateTime QueryDate { get; set; }
        [Display(Name = "Customer")]
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        [Display(Name = "Contact")]
        [ForeignKey("Contact")]
        public int? ContactID { get; set; }
        public virtual Contact Contact { get; set; }
        [Display(Name = "Service Contract")]
        [ForeignKey("ServiceContract")]
        public int? ServiceContractID { get; set; }
        public virtual ServiceContract ServiceContract { get; set; }
        [Display(Name = "Warranty Contract")]
        [ForeignKey("WarrantyContract")]
        public int? WarrantyContractID { get; set; }
        public virtual WarrantyContract WarrantyContract { get; set; }
        [Display(Name = "Query Description")]
        [Required]
        [StringLength(500)]
        public string QueryDescription { get; set; }
        [Display(Name = "Cancel Reason")]
        [StringLength(100)]
        public string LostReason { get; set; }
        #endregion
        #region Job
        [Display(Name="Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM, yy}")]
        public DateTime? JobCardCreateDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM, yy}")]
        [Display(Name = "Finish Date")]
        public DateTime? JobFinishedDate { get; set; }
        public string JobNo { get; set; }
        public string BillNo { get; set; }
        [ForeignKey("Wing")]
        [Display(Name="Revenue Wing")]
        public int? WingID { get; set; }
        public virtual RevenueWing Wing { get; set; }
        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }
        [Display(Name = "Service Charge")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ServiceCharge { get; set; }
        [Display(Name = "Workshop Charge")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WorkshopCharge { get; set; }
        [StringLength(100)]
        public string AttachmentURL { get; set; }
        public bool IsTour { get; set; }
        #endregion

        #region Derive
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Expense
        {
            get
            {
                if (ManpowerCosts.Count == 0)
                    return 0;
                return ManpowerCosts.Sum(m=>m.TotalExpense);
            }
        }
        public string SearchText
        {
            get
            {
                return JobID+" "+QueryDescription + " " +Tour+" "+
                    (Customer ?? new Customer()).Name+" "+JobTitle+" "+QueryType+" "+QueryFor+" "+
                    WingName+" "+Status+" "+State+" "+ProductNames+" "+ServicePeople+" "+JobNo+" "+BillNo;
            }
        }

        public string WingName
        {
            get
            {
                return (Wing ?? new RevenueWing()).Name;
            }
        }
        public string ProductNames
        {
            get
            {
                if (Products.Count == 0)
                    return "";
                string pn = "";
                foreach(var p in Products.Select(pr=>pr.Product.ProductName).Distinct())
                {
                    pn = pn + p+ "<br/>";
                }
                return pn;
            }
        }
        public string ServicePeople
        {
            get
            {
                if (ManpowerCosts.Count == 0)
                    return "";
                string pn = "";
                foreach (var mc in ManpowerCosts)
                {
                    pn = pn + mc.Employee.DisplayText + "<br/>";
                }
                return pn;
            }
        }
        public string StrQueryType 
        { 
            get
            {
                return Enum.GetName(typeof(EQueryType), QueryType);
            }
        }
        public string StrQueryFor
        {
            get
            {
                return Enum.GetName(typeof(EQueryFor), QueryFor);
            }
        }
        public string Tour
        {
            get
            {
                return (IsTour == true ? "tour" : "");
            }
        }
        #endregion
        
    }
}