using SMS.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class SC_Sales:EntryInfo
    {
        [Key]
        public string Invoice_Doc { get; set; }
        public EBusinessUnit BU { get; set; }
        public DateTime Invoice_Date { get; set; }
        public string Profit_Center { get; set; }
        public string Customer_ID { get; set; }
        public string Customer_Name { get; set; }
        public decimal Revenue { get; set; }
        public decimal Vat { get;set;}
        public string SM_ID { get; set; }
        public int SMUID { get; set; }
        public string SM_Name { get; set; }
        public string SP_ID { get; set; }
        public int SPUID { get; set; }
        public string SP_Name { get; set; }
        [NotMapped]
        public string ASE_ID { get; set; }
        [NotMapped]
        public string ASE_NAME { get; set; }
        public decimal Spare_Revenue { get; set; }
        public decimal Spare_Vat { get; set; }
        public decimal Spare_Perc { get; set; }
        public decimal Service_Revenue { get; set; }
        public decimal Service_Vat { get; set; }
        public decimal Lube_Revenue { get; set; }
        public decimal Lube_Vat { get; set; }
        public decimal Lube_Perc { get; set; }
        public decimal Lube_Qty { get; set; }
        public decimal ASELubeQty
        {
            get
            {
                return (ASE_Perc * Lube_Qty) / 100;
            }
        }
        public decimal ASE_Portion { get; set; }




        public string Sale_Type
        {
            get
            {
                return Service_Revenue > 0 ? "SERVICE" : "SPARE";
            }
        }
        
        public virtual ICollection<SC_Collection> Collections { get; set; }
        public virtual ICollection<SC_ASEContribution> Contributions { get; set; }
        [NotMapped]
        public VMCont VMCont { get; set; }

        public decimal ASE_Perc
        {
            get
            {
                if(SaleAmount==0)
                    return 0;
                return (100 * ASE_Portion) / SaleAmount;
            }
        }
        public decimal TotalCollection
        {
            get
            {
                if(Collections.Count>0)
                {
                    return Collections.Sum(m => m.AMOUNT);
                }
                return 0;
            }
        }

        public decimal TotalCollectionSpare
        {
            get
            {
                if (Collections.Count > 0)
                {
                    return Collections.Where(x => x.SALE_TYPE == "SPARE").Sum(m => m.AMOUNT);
                }
                return 0;
            }
        }

        public decimal TotalSpareContribution
        {
            get
            {
                if (Contributions.Count > 0)
                {
                    return Contributions.Sum(x => x.ContrAmount);
                }
                else
                    return 0;
            }
        }

        public bool IslastPayment
        {
            get
            {
                if(Collections.Count>0)
                {
                    return Collections.Where(x => x.SALE_TYPE == "SPARE").Select(x => x.IsLastPayment).FirstOrDefault();
                }
                return false;
            }
        }

        public decimal ASECollection
        {
            get
            {
                return (ASE_Perc * TotalCollection) / 100;
            }
        }
        public decimal SpareCollection
        {
            get
            {
                return (Spare_Perc * TotalCollection) / 100;
            }
        }

        public decimal SaleAmount
        {
            get
            {
                return Revenue + Vat;
            }
        }

        public decimal ServiceAmount
        {
            get
            {
                return Service_Revenue + Service_Vat;
            }
        }

        public decimal SpareAmount
        {
            get
            {
                return Spare_Revenue + Spare_Vat;
            }
        }
        public decimal ASESpareAmount
        {
            get
            {
                return (ASE_Perc * SpareAmount) / 100;
            }
        }

        public decimal LubeAmount
        {
            get
            {
                return Lube_Revenue + Lube_Vat;
            }
        }

        public decimal ASELubeAmount
        {
            get
            {
                return (ASE_Perc * LubeAmount) / 100;
            }
        }

        public SC_Sales()
        {
            Collections = new HashSet<SC_Collection>();
            Contributions=new HashSet<SC_ASEContribution>();
        }

        public string SearchText
        {
            get
            {
                return Invoice_Doc + " " + Customer_ID + " " + Customer_Name + " " + SM_ID + " " + SM_Name + " " + SP_ID + " " + SP_Name+" "+Sale_Type;
            }
            
        }
        public string CustomerDisplay
        {
            get
            {
                return Customer_ID + "--" + Customer_Name;
            }
        }
        public string SMDisplay
        {
            get
            {
                return SM_ID + "--" + SM_Name;
            }
        }
        public string SPDisplay
        {
            get
            {
                return SP_ID + "--" + SP_Name;
            }
        }

        public string ASEDisplay
        {
            get
            {
                string names = "";
                if (Contributions.Count > 0)
                {
                    UserRepository urep = new UserRepository();
                    foreach (var cont in Contributions)
                    {
                        var ase = urep.Get(cont.ASEID);
                        if (ase != null)
                        {
                            names = names + "| " + ase.DisplayText;
                        }
                    }
                }
                return names;
            }
            
        }
    }
}