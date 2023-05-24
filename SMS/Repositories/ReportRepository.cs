using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMS.Interfaces;
using SMS.Models;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace SMS.Repositories
{
    public class ReportRepository:IReportRepository
    {
        #region Properties
        private SMSContext db { get; set; }
        private UserRepository user { get; set; }
        private JobRepository job { get; set; }
        private ContractRepository contract { get; set; }
        private SalesCommissionRepository salcom { get; set; }
        private ConditionMatrixRepository condition { get; set; }
      
        public ReportRepository(SMSContext _db,UserRepository _user,
            JobRepository _job,ContractRepository _contract,SalesCommissionRepository _salecom,
            ConditionMatrixRepository _condition)
        {
            db = _db;
            user = _user;
            job = _job;
            contract = _contract;
            salcom = _salecom;
            condition = _condition;
        }
        public ReportRepository()
            : this(new SMSContext(),new UserRepository(),
            new JobRepository(),new ContractRepository(),new SalesCommissionRepository(),new ConditionMatrixRepository())
        {

        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Service Management Methods
        public PerformanceModel GetIndividualPerformance(DateTime Start,DateTime End,int UID)
        {
            var rows = db.ManpowerCosts.Where(mh => mh.StartTime >= Start
                && mh.StartTime <= End && mh.EmployeeID == UID).AsEnumerable();
            if (rows.Count() == 0)
                return new PerformanceModel();
            var result = new PerformanceModel
            {
                ServicePeople = user.Get(UID),
                PendingJobs = rows.Where(mh => mh.Job.State == EState.Pending).Select(mh=>mh.JobID).Distinct().Count(),
                CompleteJobs = rows.Where(mh => mh.Job.State == EState.Finished).Select(mh => mh.JobID).Distinct().Count(),
                WH = rows.Sum(mh => mh.WorkingHour),
                OTH = rows.Sum(mh => mh.OTHour),
                HH = rows.Sum(mh => mh.HolidayHour),
                TH = rows.Sum(mh => mh.TravelHour),
                WHC = rows.Sum(mh => (mh.WorkingHour * mh.WorkingHourRate)),
                OTHC = rows.Sum(mh => (mh.OTHour * mh.OTHourRate)),
                HHC = rows.Sum(mh => (mh.HolidayHour * mh.HolidayHourRate)),
                FA = rows.Sum(mh => (mh.NumberOfMeal*mh.FoodAllowance)),
                TC = rows.Sum(mh => mh.TransportCost),
                TourDay=rows.Sum(mh=>mh.NumberOfDay),
                TourAllowance=rows.Sum(mh=>((mh.NumberOfDay*mh.DailyAllowance)
                    +(mh.NumberOfDay*mh.Accomodation))),
                Expense = rows.Sum(mh => mh.TotalExpense)
            };
            return result;
        }

        public IEnumerable<PerformanceModel> GetTeamPerformance(DateTime Start, DateTime End, int UID)
        {
            var allperformance = from row in db.ManpowerCosts.Where(s => s.Job.JobCardCreateDate >= Start && s.Job.JobCardCreateDate <= End).AsEnumerable()
                                 group row by new { row.Employee } into g
                                 select new PerformanceModel
                                 {
                                     ServicePeople=g.Key.Employee,
                                     PendingJobs=g.Where(mh=>mh.Job.State==EState.Pending).Select(mh=>mh.JobID).Distinct().Count(),
                                     CompleteJobs = g.Where(mh => mh.Job.State == EState.Finished).Select(mh => mh.JobID).Distinct().Count(),
                                     WH = g.Sum(mh => mh.WorkingHour),
                                     OTH = g.Sum(mh => mh.OTHour),
                                     HH = g.Sum(mh => mh.HolidayHour),
                                     TH = g.Sum(mh => mh.TravelHour),
                                     WHC = g.Sum(mh => (mh.WorkingHour * mh.WorkingHourRate)),
                                     OTHC = g.Sum(mh => (mh.OTHour * mh.OTHourRate)),
                                     HHC = g.Sum(mh => (mh.HolidayHour * mh.HolidayHourRate)),
                                     FA = g.Sum(mh => (mh.NumberOfMeal * mh.FoodAllowance)),
                                     TC = g.Sum(mh => mh.TransportCost),
                                     TourAllowance=g.Sum(mh=>((mh.NumberOfDay*mh.DailyAllowance)+(mh.NumberOfDay*mh.Accomodation))),
                                     TourDay=g.Sum(mh=>mh.NumberOfDay),
                                     Expense = g.Sum(mh => mh.TotalExpense)
                                 };
            var userids = user.GetAllUser(UID,new List<int>());
            var performance = from row in allperformance
                              join prow in userids on row.ServicePeople.UserID
                              equals prow
                              select row;
            return performance.OrderByDescending(p=>p.Expense);
        }
        public IEnumerable<BarChartModel> GetRevenueExpenseByProduct(DateTime Start, DateTime End, int UID)
        {
            var userids = user.GetAllUser(UID, new List<int>());
            var allpl = from row in db.ManpowerCosts.Where(j => (j.Job.State == EState.Pending) || (j.Job.State == EState.Finished))
                        .Where(j => j.Job.JobCardCreateDate >= Start && j.Job.JobCardCreateDate <= End).AsEnumerable()
                        join prow in userids on row.EmployeeID
                        equals prow
                        join nrow in db.ProductLines on row.JobID equals nrow.JobID
                        select new { 
                            Product=nrow.Product,
                            OncallCharge=nrow.Job.ServiceCharge,
                            ServiceCharge=nrow.Job.QueryFor==EQueryFor.ServiceContract?
                            nrow.Job.ServiceContract.TotalCollectedAmount(Start,End):0,
                            WarrantyCharge=nrow.Job.QueryFor==EQueryFor.Warranty?
                            nrow.Job.WarrantyContract.TotalServiceAmount:0,
                            Expense=row.TotalExpense
                        };
            var result = from row in allpl
                         group row by new { row.Product } into g
                         select new BarChartModel
                         {
                             Series1=g.Key.Product.ProductName,
                             Value1=g.Sum(pl=>pl.OncallCharge)+
                             g.Sum(pl=>pl.ServiceCharge)+g.Sum(pl=>pl.WarrantyCharge),
                             Value4=g.Sum(pl=>pl.Expense)
                         };
            return result;
        }
        public IEnumerable<BarChartModel> GetRevenueExpenseByWing(DateTime Start, DateTime End, int UID)
        {
            var userids = user.GetAllUser(UID, new List<int>());
            var alljobs = from row in db.ManpowerCosts.Where(j => (j.Job.State == EState.Pending) || 
                        (j.Job.State == EState.Finished))
                        .Where(j => j.Job.JobCardCreateDate >= Start && j.Job.JobCardCreateDate <= End).AsEnumerable()
                        join prow in userids on row.EmployeeID equals prow
                        select new
                        {
                            Wing = row.Job.Wing,
                            OncallCharge = row.Job.ServiceCharge,
                            ServiceCharge = row.Job.QueryFor == EQueryFor.ServiceContract ?
                            row.Job.ServiceContract.TotalCollectedAmount(Start, End) : 0,
                            WarrantyCharge = row.Job.QueryFor == EQueryFor.Warranty ?
                            row.Job.WarrantyContract.TotalServiceAmount : 0,
                            Expense = row.TotalExpense
                        };
            var result = from row in alljobs
                         group row by new { row.Wing } into g
                         select new BarChartModel
                         {
                             Series1 = g.Key.Wing.Name,
                             Value1 = g.Sum(pl => pl.OncallCharge) +
                             g.Sum(pl => pl.ServiceCharge) + g.Sum(pl => pl.WarrantyCharge),
                             Value4 = g.Sum(pl => pl.Expense)
                         };
            return result;
        }
        public IEnumerable<BarChartModel> GetTopContractByProfit(DateTime Start, DateTime End, int UID)
        {
            var contracts = from row in job.GetAssignedJob(UID).Where(j => j.QueryFor == EQueryFor.ServiceContract
                                && j.JobCardCreateDate >= Start && j.JobCardCreateDate <= End)
                            group row by new { row.ServiceContract } into g
                            select new BarChartModel
                            {
                                Series1 = g.Key.ServiceContract.Customer.DisplayText,
                                Value1 = g.Sum(j => j.ServiceContract.TotalCollectedAmount(Start, End)),
                                Value2 = g.Sum(j => j.Expense),
                                Value3 = g.Count()
                            };
            return contracts.OrderByDescending(sc=>(sc.Value1-sc.Value2)).Take(15);
        }
        public IEnumerable<BarChartModel> GetTopContractByLoss(DateTime Start, DateTime End, int UID)
        {
            var contracts = from row in job.GetAssignedJob(UID).Where(j => j.QueryFor == EQueryFor.ServiceContract
                                && j.JobCardCreateDate >= Start && j.JobCardCreateDate <= End)
                            group row by new { row.ServiceContract } into g
                            select new BarChartModel
                            {
                                Series1 = g.Key.ServiceContract.Customer.DisplayText,
                                Value1 = g.Sum(j => j.ServiceContract.TotalCollectedAmount(Start, End)),
                                Value2 = g.Sum(j => j.Expense),
                                Value3 = g.Count()
                            };
            return contracts.OrderByDescending(sc => (sc.Value2 - sc.Value1)).Take(15);
        }
        public IEnumerable<BarChartModel> GetMostComplainedWarrantyProduct(DateTime Start, DateTime End, int UID)
        {
            var products = from row in job.GetAssignedJob(UID).Where(j => j.QueryFor == EQueryFor.Warranty
                                && j.QueryType == EQueryType.Complain
                                && j.JobCardCreateDate >= Start && j.JobCardCreateDate <= End)
                            join prow in db.ProductLines on row.JobID equals prow.JobID
                            group prow by new {prow.WarrantyProductID} into g
                            select new BarChartModel
                            {
                                Series1 = db.WarrantyProducts.Find(g.Key.WarrantyProductID).Product.ProductName,
                                Series2=db.WarrantyProducts.Find(g.Key.WarrantyProductID).Property,
                                Series3=db.WarrantyProducts.Find(g.Key.WarrantyProductID).WarrantyContract.Customer.DisplayText,
                                Value1 =g.Count()
                            };
            return products.Where(p=>p.Value1>0);
        }
        public IEnumerable<BarChartModel> SPCloseWithDays(UserInfo UI)
        {

            var contracts = from row in contract.GetSCAll(UI).Where(sc => sc.Status == EStatus.Open).AsEnumerable()
                            select new BarChartModel
                            {
                                Series1 = row.Customer.Name+" - "+row.Products,
                                Series2 = row.Customer.Address,
                                Series3 = row.ContractStartDate.ToString("dd MMM, yy"),
                                Series4 = row.ContractEndDate.ToString("dd MMM, yy"),
                                Value1 = (decimal)(row.ContractEndDate - DateTime.Today).TotalDays+1,
                                Value2=row.TotalCollectedAmount(null,null)
                            };
            return contracts.Where(c => c.Value1 < 31).OrderBy(c=>c.Value1);

        }
        public IEnumerable<BarChartModel> WPExpireWithDays(UserInfo UI)
        {
            var contracts = from row in contract.GetWCAll(UI).Where(sc => sc.Status == EStatus.Open).AsEnumerable()
                            select new BarChartModel
                            {
                                Series1 = row.Customer.Name + " - " + row.Products,
                                Series2 = row.Customer.Address,
                                Series3 = row.WarrantyStart.ToString("dd MMM, yy"),
                                Series4 = row.WarrantyEnd.ToString("dd MMM, yy"),
                                Value1 = (decimal)(row.WarrantyEnd - DateTime.Today).TotalDays+1,
                                Value2 = row.TotalServiceAmount
                            };
            return contracts.Where(c => c.Value1 < 31).OrderBy(c => c.Value1);
        }
        public IEnumerable<ManpowerCost> GetJobHistory(DateTime Start, DateTime End, int UID)
        {
            return db.ManpowerCosts.Where(mc => mc.Job.JobCardCreateDate >= Start &&
                mc.Job.JobCardCreateDate <= End && mc.EmployeeID == UID);
        }

        public IEnumerable<ManpowerCost> GetJobHistoryInDateRange(DateTime Start, DateTime End)
        {
            return db.ManpowerCosts.Where(mc => mc.Job.JobCardCreateDate >= Start &&
                mc.Job.JobCardCreateDate <= End);
        }
        public IEnumerable<BarChartModel> GetJobByProductProperty(DateTime Start, DateTime End, int UID,int PPID)
        {
            var userids = user.GetAllUser(UID, new List<int>());
            var allpld = from row in db.ManpowerCosts.Where(j => (j.Job.State == EState.Pending) || (j.Job.State == EState.Finished))
                        .Where(j => j.Job.JobCardCreateDate >= Start && j.Job.JobCardCreateDate <= End).AsEnumerable()
                        join prow in userids on row.EmployeeID
                        equals prow
                        join nrow in db.ProductLines on row.JobID equals nrow.JobID
                        join dnrow in db.ProductDetails.Where(pp=>pp.ProductPropertyID==PPID) on nrow.ProductLineID equals dnrow.ProductLineID
                        select dnrow;
            var result = from row in allpld
                         group row by new { row.Value } into g
                         select new BarChartModel
                         {
                             Series1 = g.Key.Value,
                             Value1 = g.Count()
                         };
            return result.OrderByDescending(c=>c.Value1);
        }
        public IEnumerable<Job> GetJobHistoryByCustomer(DateTime Start, DateTime End, int CID)
        {
            return db.Jobs.Where(j => j.QueryDate >= Start
                && j.QueryDate <= End && j.CustomerID == CID);
        }




        #endregion

        #region ASE Monthly Salary with Commision 

        public IEnumerable<SC_ASEApprovedData> GetASEApprovedData(UserInfo UI, DateTime start, DateTime End, int ASEID)
        {
            dynamic result = null;
            var userids = user.GetAllUser(UI.User.UserID, new List<int>());

            if (ASEID == 0)
            {
                result = from row in db.SC_ASEApprovedData
                         where row.startDate >= start && row.endDate <= End
                         && userids.Contains(row.ASEID)
                         && row.status == 3
                         select row;
            }
            else
            {
                result = from row in db.SC_ASEApprovedData
                         where row.startDate >= start && row.endDate <= End
                         && row.ASEID == ASEID
                         && row.status == 3
                         select row;
            }
           
            return result;
        }

        #endregion


        #region ASE Sales Commission
        //public IEnumerable<SC_SpareCom> SC_GetSpareSaleCollection(int ASEUID, DateTime Start, DateTime End, int days)
        //{
        //    var AllCollection=(from row in salcom.GetAllCollection()
        //                      join prow in salcom.GetAllAC()
        //                      on row.INVOICE_DOC equals prow.Invoice_Doc
        //                      where row.PAYMENT_DATE >= Start && row.PAYMENT_DATE<=End
        //                      && prow.ASEID ==ASEUID&&row.SALE_TYPE=="SPARE"
        //                      select new 
        //                      {
        //                          InvoiceNo=row.INVOICE_DOC,
        //                          ASE=row.ASE,
        //                          Customer=row.CustomerDisplay,
        //                          ASEContr=prow.ContrAmount,
        //                          ASEPortion=prow.Sale.ASE_Portion,
        //                          SparePerc=prow.Sale.Spare_Perc,
        //                          SpareCollection=(row.AMOUNT*prow.Sale.Spare_Perc)/100,
        //                          SpareAmount=prow.Sale.SpareAmount,
        //                          InvoiceDate=prow.Sale.Invoice_Date,
        //                          PaymentDate=row.PAYMENT_DATE,
        //                          IsLast=row.IsLastPayment
                                  
        //                      });
        //    var InvoiceCollection=from row in AllCollection
        //                          group row by (row.InvoiceNo) into g
        //                          select new 
        //                          {
        //                              InvoiceNo=g.Key,
        //                              Collection=g.Select(c=>c.SpareCollection).DefaultIfEmpty(0).Sum()
        //                          };
        //    var ValidCollection = new List<SC_SpareCom>();
        //    SC_SpareCom ColDetail;
        //    SC_ASETarget Target;
        //    foreach(var _col in InvoiceCollection)
        //    {
        //        var col=AllCollection.Where(c=>c.InvoiceNo==_col.InvoiceNo).FirstOrDefault();
        //        if((col.PaymentDate-col.InvoiceDate).Days<=days)
        //        {
        //            if(AllCollection.Any(c=>c.InvoiceNo==_col.InvoiceNo&&c.IsLast==true))
        //            {
        //                ColDetail = new SC_SpareCom();
        //                Target = new SC_ASETarget();

                             
        //            }
        //            else if(col.SpareAmount==_col.Collection)
        //            {

        //            }
        //        }
        //    }
        //    return ValidCollection;
        //}
        #endregion
        public void Save()
        {
            db.SaveChanges();
        }
    }
}