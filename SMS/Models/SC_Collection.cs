using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class SC_Collection : EntryInfo
    {
        public int ID { get; set; }
        public DateTime PAYMENT_DATE { get; set; }
        public string PAYMENT_DOC { get; set; }
        public int FISCAL_YEAR { get; set; }
        public EBusinessUnit BU { get; set; }
        public string PROFIT_CENTER { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        [ForeignKey("Sale")]
        public string INVOICE_DOC { get; set; }
        public virtual SC_Sales Sale { get; set; }
        public string SM_ID { get; set; }
        public int SMUID { get; set; }
        public string SM_NAME { get; set; }
        public string SP_ID { get; set; }
        public int SPUID { get; set; }
        public string SP_NAME { get; set; }
        [ForeignKey("ASE")]
        public int ASEUID { get; set; }
        public virtual User ASE { get; set; }
        public string PMT_TYPE { get; set; }
        public bool IsLastPayment { get; set; }
        public bool? isApproved { get; set; }

        public string DOC_TYPE { get; set; }
        public string SALE_TYPE { get; set; }

        public int? status { get; set; }

        public int? rejectBy { get; set; }
        public DateTime? rejectDateTime { get; set; }
     
        public int? assignBy { get; set; }
        public string assignDisplayName { get; set; }
        //public string  getFullName
        //{
        //    get
        //      {
        //        return "";
        //      }
        //}

         
        
        public string SearchText
        {
            get
            {
                return PAYMENT_DOC + " " + INVOICE_DOC + " " + CUSTOMER_ID + " " + CUSTOMER_NAME + " " + SM_ID + " " + SM_NAME + " " + SP_ID + " " + SP_NAME + " " + SALE_TYPE + " " + DOC_TYPE + " " + AMOUNT + " " + assignDisplayName;
            }

        }

        public string CustomerDisplay
        {
            get
            {
                return CUSTOMER_ID + "--" + CUSTOMER_NAME;
            }
        }

        public string SMDisplay
        {
            get
            {
                return SM_ID + "--" + SM_NAME;
            }
        }
        public string SPDisplay
        {
            get
            {
                return SP_ID + "--" + SP_NAME;
            }
        }

        

    }
}