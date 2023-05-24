using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class SC_ASEReport
    {
        public DateTime InvoiceDate { get; set; }
        public DateTime CollectionDate { get; set; }
        public int Duration { get; set; }
     
        public string Invoice_Doc { get; set; }
        public string PAYMENT_DOC { get; set; }
        public string Customer_Name { get; set; }
        public decimal Revenue { get; set; }
        public decimal ASE_Portion { get; set; }

        public decimal Claimable_ASE_Collection { get; set; }

        public decimal Collections { get; set; }
        public decimal SaleAmt { get; set; }
        public decimal SpareSalesAmt { get; set; }

        public decimal CollectionPerAmt { get; set; }
        public decimal Bonus { get; set; }

        public string ASEDisplayName { get; set; }

        public decimal lubeQty { get; set; }
        public decimal serviceBonus { get; set; }

        public string SaleType { get; set; }

        public string Eligible { get; set; }
        public int? ASEID { get; set; }

        public List<SC_ASEReportTDSVDS> TDS_VDS { get; set; }
    }
}