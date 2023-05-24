using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class SPService:EntryInfo
    {
        public int ID { get; set; }
        public EBusinessUnit BU { get; set; }
        #region Fixed Member
        [ForeignKey("SAPProduct"),Column(Order=0)]
        public string DocNo { get; set; }
        [ForeignKey("SAPProduct"), Column(Order = 1)]
        public int ItemNo { get; set; }
        public virtual SAPProduct SAPProduct { get; set; }
        #endregion
        public DateTime JobCompletionDate { get; set; }
        public EQueryType CustomerStatus { get; set; }
        public EJobCategory JobCategory { get; set; }
        public int RunningHour { get; set; }

        [ForeignKey("ServicePerson")]
        public int ServicePersonID { get; set; }
        public virtual User ServicePerson { get; set; }
        public string Attachment { get; set; }
        public bool Cancel { get; set; }

        public virtual ICollection<SPServiceDetail> Details { get; set; }
        

        public SPService()
        {
            Details = new HashSet<SPServiceDetail>();
        }

        public string WorkDetail
        {
            get
            {
                string ret = "";
                if(Details.Count>0)
                {
                    foreach(var det in Details.Where(c=>c.IsDone==true))
                    {
                        if(det.WorkID==118)
                        {
                            ret = ret + det.ServiceDetail + ", ";
                        }
                        else
                        {
                            ret = ret + det.ServiceWork.WorkName + ", ";
                        }
                        
                    }
                }
                return ret;
            }
        }

        public string SearchText
        {
            get
            {
                return ID + " " + DocNo + " " + (SAPProduct ?? new SAPProduct()).Brand + " " + (SAPProduct ?? new SAPProduct()).CUSTOMERCODE + " " +
                    (SAPProduct ?? new SAPProduct()).CUSTOMERNAME + " " + (SAPProduct ?? new SAPProduct()).ENGINESERIAL + " " +
                    (SAPProduct ?? new SAPProduct()).GRP + " " + (SAPProduct ?? new SAPProduct()).GRPNAME + " " + (SAPProduct ?? new SAPProduct()).MATERIALCODE +
                    " " + (SAPProduct ?? new SAPProduct()).PROFITCENTER + " " + (SAPProduct ?? new SAPProduct()).Rating + " " + (SAPProduct ?? new SAPProduct()).UII+" "+
                    CustomerStatus + " " + JobCategory + " " + (ServicePerson ?? new User()).Department + " " + (ServicePerson ?? new User()).FullName + " " + 
                    (ServicePerson ?? new User()).UserName;
            }
        }

    }
}