using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class SAPProduct
    {
        [Key,Column(Order=0)]
        [StringLength(8)]
        public string DOCNO { get; set; }
        [Key, Column(Order = 1)]
        public int ITEMNO { get; set; }
        public string ENGINESERIAL { get; set; }
        public DateTime DELIVERYDATE { get; set; }
        public DateTime GRDATE { get; set; }
        public string CUSTOMERCODE { get; set; }
        public string CUSTOMERNAME { get; set; }
        public string ADDRESS { get; set; }
        public string MATERIALCODE { get; set; }
        public string MATERIALNAME { get; set; }
        public string PROFITCENTER { get; set; }
        public string SALESORG { get; set; }
        public string DOCTYPE { get; set; }
        public string UII { get; set; }
        [ForeignKey("PGC")]
        public string GRP { get; set; }
        public virtual ProductGrpCat PGC { get; set; }
        public string GRPNAME { get; set; }
        #region Modifyable Fields
        public DateTime CommissioningDate { get; set; }
        public string Location { get; set; }
        public string Brand { get; set; }
        public string EQModel { get; set; }
        public string EQSerial { get; set; }
        public string Rating { get; set; }

        #endregion

        public string Url
        {
            get
            {
                return "http://service.energypac.com/SPService/NewService?DocNo=" + DOCNO+"&ItemNo="+ITEMNO;
            }
        }

        public string FormatSerial
        {
            get
            {
                var serial = ENGINESERIAL;
                if((GRP??"")!=""&&GRP=="FG25")
                {
                    var split = ENGINESERIAL.Split('-');
                    if(split.Count()>1)
                    {
                        serial = split[1];
                    }
                }
                return serial;
            }
        }

        public string MaterialDisplay
        {
            get
            {
                return MATERIALCODE + "-" + MATERIALNAME;
            }
        }

        public string CustomerDisplay
        {
            get
            {
                return CUSTOMERCODE + "-" + CUSTOMERNAME;
            }
        }

        public string SearchText
        {
            get
            {
                return ENGINESERIAL + " " + DOCNO + " " + CUSTOMERCODE + " " + CUSTOMERNAME + " " + UII+" "+MATERIALCODE+" "+MATERIALNAME+" "+
                    PROFITCENTER+" "+" "+(PGC??new ProductGrpCat()).Brand+
                    ((PGC??new ProductGrpCat()).Product??new Product()).ProductName;
            }
        }
    }
}