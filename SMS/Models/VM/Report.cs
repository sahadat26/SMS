using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class ReportParameter
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int? CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public int? ProductID { get; set; }
        public int? PropertyID { get; set; }
        public int ReportID { get; set; }

    }

    public class SC_ReportParameter
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int SalesRange { get; set; }
        public int? ASEID { get; set; }
        public string Month { get; set; }
        public int ReportID { get; set; }

    }
    public class PerformanceModel
    {
        public User ServicePeople { get; set; }
        public int QueryReceived { get; set; }
        public int PendingJobs { get; set; }
        public int CompleteJobs { get; set; }
        public decimal WH { get; set; }
        public decimal OTH { get; set; }
        public decimal TH { get; set; }
        public decimal HH { get; set; }
        public decimal WHC { get; set;}
        public decimal OTHC { get; set; }
        public decimal HHC { get; set; }
        public decimal FA { get; set; }
        public decimal TC { get; set; }
        public int TourDay { get; set; }
        public decimal TourAllowance { get; set; }
        public decimal Expense { get; set; }
    }
    public class BarChartModel
    {
        public string Series1 { get; set; }
        public string Series2 { get; set; }
        public string Series3 { get; set; }
        public string Series4 { get; set; }
        public decimal Value1 { get; set; }
        public decimal Value2 { get; set; }
        public decimal Value3 { get; set; }
        public decimal Value4 { get; set; }
        
    }
    public class SC_SpareCollection
    {
        public int ASEUID { get; set; }
        public string Customer { get; set; }
        public EConditionType ConditionType { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal CollectedAmount { get; set; }
    }

    public class SC_AllCommission
    {
        public int ASEUID { get; set; }
        public User ASE { get; set; }
        public ICollection<SC_SpareCollection> SpareCollection { get; set; }
        public SC_AllCommission()
        {
            SpareCollection = new HashSet<SC_SpareCollection>();
        }
        public SC_ConditionMatrix SpareCondition { get; set; }
        public decimal TotalCollection
        {
            get
            {
                if(SpareCollection.Count>0)
                {
                    return SpareCollection.Select(c => c.CollectedAmount).DefaultIfEmpty(0).Sum();
                }
                return 0;
            }
        }
        public decimal SpareCommission
        {
            get
            {
                if(SpareCondition!=null)
                {
                    return (TotalCollection * SpareCondition.Perc) / 100;
                }
                return 0;
            }
        }
        public decimal SpareBonus
        {
            get
            {
                if (SpareCondition != null)
                {
                    return SpareCondition.Bonus;
                }
                return 0;
            }
        }
        
    }
}