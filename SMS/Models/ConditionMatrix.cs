using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class SC_ConditionMatrix:EntryInfo
    {
        public int ID { get; set; }
        public EBusinessUnit BU { get; set; }
        public EConditionType ConditionType { get; set; }
        public int SL { get; set; }
        public decimal RangeStart { get; set; }
        public decimal RangeEnd { get; set; }
        public decimal Perc { get; set; }
        public decimal Bonus { get; set; }
        public decimal FixedAmount { get; set; }
        public EContractType ContractType { get; set; }
        public int LtrQty { get; set; }

        public string SearchText
        {
            get
            {
                return ID+" "+ Enum.GetName(typeof(EConditionType), ConditionType) + " " + Enum.GetName(typeof(EContractType), ContractType);
            }
        }
    }
}