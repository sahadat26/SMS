using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using SMS.Interfaces;
using SMS.Reports;
using DevExpress.XtraReports.UI;

namespace SMS.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/
        private IReportRepository report { get; set; }
        private IUserRepository user { get; set; }
        private IProductRepository product{get;set;}
        private ICustomerRepository customer { get; set; }
        
        public ReportController(IReportRepository _report,
            IUserRepository _user,IProductRepository _product,
            ICustomerRepository _customer)
        {
            report = _report;
            user = _user;
            product = _product;
            customer = _customer;
        }
       
        [CustomAuthorize]
        public ActionResult Index()
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            if(TempData["Error"]!=null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Info = TempData["Info"];
            }
            ViewBag.Employees = user.GetSelectListByUser(UI.User.UserID);
            ViewBag.Products = product.GetProductSelectList(UI.BusinessUnit);
            ViewBag.Customers = customer.GetSelectListByAssignedJob(UI);
            if(user.CheckRoleExistant(UI.User.UserID,"Report"))
            {
                ViewBag.Employees = user.GetSelectList(UI);
                ViewBag.Customers = customer.GetSelectListByJob(UI);
            }
            return View(new ReportParameter { Start=DateTime.Today.AddMonths(-3),End=DateTime.Today});
        }

        [CustomAuthorize(Roles = "Admin, SaleCom")]
        public ActionResult SC_Index()
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Info = TempData["Info"];
            }
            ViewBag.ASE = user.GetASE(UI);
            var firstdate = new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
            var lastdate = firstdate.AddMonths(1).AddDays(-1);
            var month = firstdate.ToString("MMMM");
            return View(new SC_ReportParameter { Start = firstdate, End = lastdate,SalesRange=30,Month=month });
        }

        [HttpPost]
        [CustomAuthorize]
        public ActionResult PreviewReport(ReportParameter par)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                switch(par.ReportID)
                {
                    case 1:
                        {
                            if(par.EmployeeID==0)
                            {
                                throw new Exception("No Employee Selected");
                            }
                            var performance = report.GetIndividualPerformance(par.Start, par.End, par.EmployeeID.Value);
                            var emp = user.Get(par.EmployeeID.Value);
                            IndPerformance rp = new IndPerformance(performance,par.Start,par.End,emp.DisplayText);
                            return View("ReportPreview",rp);
                        }   
                    case 2:
                        {
                            if (par.EmployeeID == 0)
                            {
                                throw new Exception("No Employee Selected");
                            }
                            var lstperformance = report.GetTeamPerformance(par.Start, par.End, par.EmployeeID.Value);
                            var emp = user.Get(par.EmployeeID.Value);
                            TeamPerformance rp = new TeamPerformance(lstperformance, par.Start, par.End,emp.DisplayText);
                            return View("ReportPreview", rp);
                        }
                    case 3:
                        {
                            if (par.EmployeeID == 0)
                            {
                                throw new Exception("No Employee Selected");
                            }
                            var lstRE = report.GetRevenueExpenseByProduct(par.Start, par.End,par.EmployeeID.Value);
                            var emp = user.Get(par.EmployeeID.Value);
                            REByProduct rp = new REByProduct(lstRE, par.Start, par.End,emp.DisplayText);
                            return View("ReportPreview", rp);
                        }
                    case 4:
                        {
                            if (par.EmployeeID == 0)
                            {
                                throw new Exception("No Employee Selected");
                            }
                            var lstRE = report.GetRevenueExpenseByWing(par.Start, par.End, par.EmployeeID.Value);
                            var emp = user.Get(par.EmployeeID.Value);
                            REByWing rp = new REByWing(lstRE, par.Start, par.End,emp.DisplayText);
                            return View("ReportPreview", rp);
                        }
                    case 5:
                        {
                            if (par.EmployeeID == 0)
                            {
                                throw new Exception("No Employee Selected");
                            }
                            var lstcontracts = report.GetTopContractByProfit(par.Start, par.End, par.EmployeeID.Value);
                            var emp = user.Get(par.EmployeeID.Value);
                            TopContracts rp = new TopContracts(lstcontracts, par.Start, par.End, emp.DisplayText);
                            return View("ReportPreview", rp);
                        }
                    case 6:
                        {
                            if (par.EmployeeID == 0)
                            {
                                throw new Exception("No Employee Selected");
                            }
                            var lstproducts = report.GetMostComplainedWarrantyProduct(par.Start, par.End, par.EmployeeID.Value);
                            var emp = user.Get(par.EmployeeID.Value);
                            MostComplainedWP rp = new MostComplainedWP(lstproducts, par.Start, par.End, emp.DisplayText);
                            return View("ReportPreview", rp);
                        }
                    case 7:
                        {
                            if (par.EmployeeID == 0)
                            {
                                throw new Exception("No Employee Selected");
                            }
                            var lstcontracts = report.GetTopContractByLoss(par.Start, par.End, par.EmployeeID.Value);
                            var emp = user.Get(par.EmployeeID.Value);
                            TopLossContracts rp = new TopLossContracts(lstcontracts, par.Start, par.End, emp.DisplayText);
                            return View("ReportPreview", rp);
                        }
                    case 8:
                        {
                            
                            var lstcontracts = report.SPCloseWithDays(UI);

                            ToBeClosed rp = new ToBeClosed(lstcontracts,"Service Contract Close in 30 Day(s)");
                            return View("ReportPreview", rp);
                        }
                    case 9:
                        {

                            var lstcontracts = report.WPExpireWithDays(UI);

                            ToBeClosed rp = new ToBeClosed(lstcontracts, "Warranty Expire Within in 30 Day(s)");
                            return View("ReportPreview", rp);
                        }
                    case 10:
                        {
                            if (par.EmployeeID == 0)
                            {
                                throw new Exception("No Employee Selected");
                            }
                            var lstmc = report.GetJobHistory(par.Start, par.End, par.EmployeeID.Value).ToList();
                            var emp = user.Get(par.EmployeeID.Value);
                            JobHistory rp = new JobHistory(lstmc, par.Start, par.End, emp.DisplayText);
                            return View("ReportPreview", rp);
                        }
                    case 11:
                        {
                            if (par.EmployeeID == 0)
                            {
                                throw new Exception("No Employee Selected");
                            }
                            else if (par.ProductID == 0)
                            {
                                throw new Exception("No Product Selected");
                            }
                            else if (par.PropertyID == 0)
                            {
                                throw new Exception("No Property Selected");
                            }
                            var lstmc = report.GetJobByProductProperty(par.Start, par.End, par.EmployeeID.Value,par.PropertyID.Value).ToList();
                            var emp = user.Get(par.EmployeeID.Value);
                            var prod = product.GetProduct(par.ProductID.Value);
                            var prop = product.GetProductProperty(par.PropertyID.Value);
                            string heading = prop.Property.Name + " wise " + prod.ProductName;
                            JobByProductProperty rp = new JobByProductProperty(lstmc, par.Start, par.End, emp.DisplayText,heading);
                            return View("ReportPreview", rp);
                        }
                    case 12:
                        {
                            if (par.CustomerID == 0)
                            {
                                throw new Exception("No Customer Selected");
                            }
                            
                            var lstjob = report.GetJobHistoryByCustomer(par.Start, par.End,par.CustomerID.Value).ToList();
                            
                            //string heading = prop.Property.Name + " wise " + prod.ProductName;
                            JobHistoryByCustomer rp = new JobHistoryByCustomer(lstjob, par.Start, par.End, "");
                            return View("ReportPreview", rp);
                        }
                    case 13:
                        {
                            
                            var lstmc = report.GetJobHistoryInDateRange(par.Start, par.End).ToList();
                            JobHistoryInDateRange rp = new JobHistoryInDateRange(lstmc, par.Start, par.End);
                            return View("ReportPreview", rp);
                        }

                    case 99:
                        {
                            var data = report.GetASEApprovedData(UI,par.Start, par.End, 0).ToList();
                            ASESalaryReport rp = new ASESalaryReport(data);
                            return View("ReportPreview", rp);
                        }
                }
            }
            catch(Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, SaleCom")]
        public ActionResult SC_PreviewReport(SC_ReportParameter par)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                switch (par.ReportID)
                {
                    case 1:
                        {
                            if (par.ASEID == 0)
                            {
                                throw new Exception("No Employee Selected");
                            }
                            var performance = report.GetIndividualPerformance(par.Start, par.End, par.ASEID.Value);
                            var emp = user.Get(par.ASEID.Value);
                            IndPerformance rp = new IndPerformance(performance, par.Start, par.End, emp.DisplayText);
                            return View("ReportPreview", rp);
                        }
                    
                }
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("Index");
        }
        #region JSON
        [HttpPost]
        [CustomAuthorize]
        public ActionResult OnChangeProduct(int PID=0)
        {
            var pro = from row in product.GetProductPropertyByProduct(PID)
                      select new
                      {
                          ID = row.ProductPropertyID,
                          Name = row.Property.Name
                      };
            return Json(pro);
            
        }
        #endregion

        #region SAP Product Service
        [CustomAuthorize]
        public ActionResult QRPrint(string[] chk,EReportBy Layout=EReportBy.ShowDeliveryDate)
        {
            if (chk != null)
            {
                var RecordFound = (from row in product.GetSAPGenset()
                                   join idrow in chk
                                   on row.ENGINESERIAL equals idrow
                                   select row).ToList();

                if (RecordFound != null)
                {
                    XtraReport rp;
                    if(Layout==EReportBy.ShowCommissioningDate)
                    {
                        rp = new rptProductQR2(RecordFound);
                        return View("ReportPreview", rp);
                    }
                    
                    rp = new rptProductQR(RecordFound);
                    return View("ReportPreview", rp);
                }
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}