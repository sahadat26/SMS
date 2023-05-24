using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using SMS.Interfaces;
using System.Threading.Tasks;
using System.Data.Entity.Validation;

namespace SMS.Controllers
{
    public class SalesCommissionController : Controller
    {
        //
        // GET: /Sales Commission/
        private ISalesCommissionRepository salecom { get; set; }
        private IUserRepository user { get; set; }
        private IUserRepository setup { get; set; }
        public SalesCommissionController(ISalesCommissionRepository _salecom, IUserRepository _setup, IUserRepository _user)
        {
            salecom = _salecom;
            setup = _setup;
            user = _user;
        }

        #region Manage Sales

        [CustomAuthorize(Roles = "Admin, SaleCom")]
        public ActionResult IndexSales(DateTime? Start, DateTime? End, string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            if (!Start.HasValue && !End.HasValue)
            {
                DateTime now = DateTime.Now;
                Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //   End = new DateTime?(DateTime.Today.AddDays(1));
                var DaysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                End = new DateTime(now.Year, now.Month, DaysInMonth);

                //Start = new DateTime?(DateTime.Today.AddDays(-90));
                //End = new DateTime?(DateTime.Today.AddDays(1));

            }
            ViewBag.StartDate = Start.Value.ToString("MM/dd/yyyy");
            ViewBag.EndDate = End.Value.ToString("MM/dd/yyyy");
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Info = TempData["Info"];
            }

            if (q.Length > 0)
            {
                var SearchResult = salecom.GetAllSales(UI)
                    .Where(c => c.Invoice_Date >= Start && c.Invoice_Date <= End)
                    .Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.Invoice_Date);

                ViewBag.SearchText = q;

                return View(SearchResult);
            }

            return View(salecom.GetAllSales(UI)
                    .Where(c => c.Invoice_Date >= Start && c.Invoice_Date <= End)
                    .OrderByDescending(e => e.Invoice_Date));
        }

        [CustomAuthorize(Roles = "Admin, SaleCom")]
        public async Task<ActionResult> SyncSales(DateTime Start, DateTime End, string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                var orgs = setup.OrgAll(UI.User.BusinessUnit);
                UI.Host = Request.ServerVariables["REMOTE_HOST"];
                foreach (var org in orgs)
                {
                    var sales = await salecom.SyncSales(org.SalesOrg, Start, End, org.Division);
                    var tobeadded = (from row in sales
                                     where !(from prow in salecom.GetAllSales() select prow.Invoice_Doc)
                                     .Contains(row.Invoice_Doc)
                                     select row).ToList();
                    foreach (var sale in tobeadded)
                    {
                        salecom.AddSale(sale, UI);
                        if ((sale.ASE_ID ?? "") != "")
                        {
                            try
                            {
                                var cont = new SC_ASEContribution();
                                cont.SL = 1;
                                cont.ASEID = setup.GetUserBySAPCode(sale.ASE_ID).UserID;
                                cont.ContrPerc = 100;
                                cont.Invoice_Doc = sale.Invoice_Doc;
                                cont.ContrAmount = sale.ASE_Portion;
                                salecom.AddAC(new List<SC_ASEContribution>() { cont }, UI);
                            }
                            catch
                            {
                                throw new Exception("Emp ID: " + sale.ASE_ID + " not found");
                            }

                        }
                    }
                    salecom.Save();


                    TempData["Info"] = "Date sucessfully synced";
                }

            }
            catch (NullReferenceException err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("IndexSales", new { Start = Start, End = End, q = q });
        }
        [CustomAuthorize(Roles = "Admin, SaleCom")]
        public ActionResult UpdateASEPortion(string InvoiceNo = "")
        {
            if (InvoiceNo == "")
            {
                TempData["Error"] = "No Invoice Selected";
                return RedirectToAction("IndexSales");
            }

            var sale = salecom.GetSale(InvoiceNo);
            var obj = new VMASEPart
            {
                ASEPartAmount = sale.ASE_Portion,
                CustomerName = sale.CustomerDisplay,
                InvoiceNo = sale.Invoice_Doc,
                SpareAmount = sale.SpareAmount,
                ASEPartPerc = sale.ASE_Perc
            };

            return PartialView("_UpdateASEPortion", obj);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, SaleCom")]
        public JsonResult UpdateASEPortion(VMASEPart obj)
        {
            try
            {
                salecom.ASEPortionUpdate(obj);
                salecom.Save();
                return Json(new { flag = "y", msg = "ASE Portion Updated" });
            }
            catch (Exception err)
            {
                return Json(new { flag = "n", msg = err.Message.ToString() });
            }
        }

        #endregion

        #region Manage Collection
        [CustomAuthorize(Roles = "Admin, SaleCom")]
        public ActionResult IndexCollections(DateTime? Start, DateTime? End, string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            if (!Start.HasValue && !End.HasValue)
            {
                DateTime now = DateTime.Now;
                //var Start = new DateTime?(now.Year, now.Month, 1);
                //var End = startDate.AddMonths(1).AddDays(-1);

                Start = new DateTime(DateTime.Now.Year,DateTime.Now.Month,1);
             //   End = new DateTime?(DateTime.Today.AddDays(1));
                var DaysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                End = new DateTime(now.Year, now.Month, DaysInMonth);

            }
            ViewBag.StartDate = Start.Value.ToString("MM/dd/yyyy");
            ViewBag.EndDate = End.Value.ToString("MM/dd/yyyy");
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Info = TempData["Info"];
            }

            if (q.Length > 0)
            {
                var SearchResult = salecom.GetAllCollection(UI)
                    .Where(c => c.PAYMENT_DATE >= Start && c.PAYMENT_DATE <= End)
                    .Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.PAYMENT_DATE);

                ViewBag.SearchText = q;

                return View(SearchResult);
            }

            return View(salecom.GetAllCollection(UI)
                    .Where(c => c.PAYMENT_DATE >= Start && c.PAYMENT_DATE <= End)
                    .OrderByDescending(e => e.PAYMENT_DATE));
        }
        [CustomAuthorize(Roles = "Admin, SaleCom")]
        public async Task<ActionResult> SyncCollections(DateTime Start, DateTime End, string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                var orgs = setup.OrgAll(UI.User.BusinessUnit);
                UI.Host = Request.ServerVariables["REMOTE_HOST"];
                foreach (var org in orgs)
                {
                    var collections = await salecom.SyncCollection(org.SalesOrg, Start, End, org.Division);
                    var tobeadded = (from row in collections
                                     where !((from prow in salecom.GetAllCollection() select prow.PAYMENT_DOC).Contains(row.PAYMENT_DOC)
                                     && (from prow in salecom.GetAllCollection() select prow.FISCAL_YEAR).Contains(row.FISCAL_YEAR)
                                     )
                                     select row).ToList();
                    foreach (var collection in tobeadded)
                    {
                        salecom.AddCollection(collection, UI);
                    }
                    salecom.Save();


                    TempData["Info"] = "Date sucessfully synced";
                }

            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("IndexCollections", new { Start = Start, End = End, q = q });
        }

        [CustomAuthorize(Roles = "Admin, SaleCom")]
        [HttpPost]
        public JsonResult SetLastPayment(int ID)
        {
            try
            {
                salecom.SetLastPayment(ID);
                salecom.Save();

                return Json(new { flag = "y", msg = "Payment Status Changed" });
            }
            catch (DbEntityValidationException err)
            {
                String errMsg = "";
                foreach (var error in err.EntityValidationErrors)
                {
                    foreach (var failure in error.ValidationErrors)
                    {
                        errMsg = failure.ErrorMessage;
                    }
                }
                return Json(new { flag = "n", msg = errMsg.ToString() });
            }
            catch (Exception err)
            {
                return Json(new { flag = "n", msg = err.Message.ToString() });
            }
        }

        [CustomAuthorize(Roles = "Admin, SaleCom")]
        public ActionResult SetColASE(int ID = 0)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            if (ID == 0)
            {
                TempData["Error"] = "No Collection Selected";
                return RedirectToAction("IndexCollections");
            }

            var collection = salecom.GetCollection(ID);
            var obj = new VMColASE
            {
                Amount = collection.AMOUNT,
                CustomerName = collection.CustomerDisplay,
                DocNo = collection.PAYMENT_DOC,
                FY = collection.FISCAL_YEAR,
                ASEUID = collection.ASEUID,
                ID = collection.ID
            };
            ViewBag.ASE = setup.GetASE(UI);
            return PartialView("_SetColASE", obj);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, SaleCom")]
        public JsonResult SetColASE(VMColASE obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                salecom.SetColASE(obj, UI);
                salecom.Save();
                return Json(new { flag = "y", msg = "Collector ASE Updated" });
            }
            catch (Exception err)
            {
                return Json(new { flag = "n", msg = err.Message.ToString() });
            }
        }

        #endregion
        #region Manage ASE Contribution
        [CustomAuthorize(Roles = "Admin, SaleCom")]
        public ActionResult IndexAC(DateTime? Start, DateTime? End, string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            if (!Start.HasValue && !End.HasValue)
            {

                Start = new DateTime?(DateTime.Today.AddDays(-90));
                End = new DateTime?(DateTime.Today.AddDays(1));

            }
            ViewBag.StartDate = Start.Value.ToString("MM/dd/yyyy");
            ViewBag.EndDate = End.Value.ToString("MM/dd/yyyy");
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Info = TempData["Info"];
            }

            if (q.Length > 0)
            {
                var SearchResult = salecom.GetAllAC(UI)
                    .Where(c => c.Sale.Invoice_Date >= Start && c.Sale.Invoice_Date <= End)
                    .Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.Sale.Invoice_Date);

                ViewBag.SearchText = q;

                return View(SearchResult);
            }

            return View(salecom.GetAllAC(UI)
                    .Where(c => c.Sale.Invoice_Date >= Start && c.Sale.Invoice_Date <= End)
                    .OrderByDescending(e => e.Sale.Invoice_Date));
        }

        [CustomAuthorize(Roles = "Admin, ACCont")]
        public ActionResult ManageAC(string Invoice = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            if (Invoice == "")
            {
                TempData["Error"] = "No Invoice Selected";
                return RedirectToAction("IndexSales");
            }
            if (!salecom.IsUserPermitted(Invoice, UI))
            {
                TempData["Error"] = "You are not permitted for invoice no: " + Invoice;
                return RedirectToAction("IndexSales");
            }
            var sale = salecom.GetSale(Invoice);
            foreach (var cont in sale.Contributions)
            {
                cont.SpareAmount = sale.ASESpareAmount;
            }
            Session["conts"] = sale.Contributions.ToList();
            sale.VMCont = new VMCont();
            sale.VMCont.Cont.Invoice_Doc = Invoice;
            sale.VMCont.Cont.SpareAmount = sale.ASESpareAmount;
            sale.VMCont.Conts = sale.Contributions.ToList();
            ViewBag.ASE = setup.GetASE(UI);
            return View(sale);

        }
        [HttpPost]
        [CustomAuthorize(Roles = "Admin, ACCont")]
        public ActionResult ManageAC(SC_Sales sale)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            UI.Host = Request.ServerVariables["REMOTE_HOST"];
            try
            {
                var conts = (List<SC_ASEContribution>)Session["conts"];
                salecom.UpdateAC(conts, UI);
                salecom.Save();
                TempData["Info"] = "ASE Contribution Saved";
                Session.Remove("conts");
                return RedirectToAction("IndexSales", new { Start = sale.Invoice_Date, End = sale.Invoice_Date, q = sale.Invoice_Doc });
            }
            catch (Exception err)
            {
                ViewBag.Error = err.Message.ToString();
            }
            ViewBag.ASE = setup.GetASE(UI);
            sale = salecom.GetSale(sale.Invoice_Doc);
            sale.VMCont = new VMCont();
            sale.VMCont.Cont.Invoice_Doc = sale.Invoice_Doc;
            sale.VMCont.Cont.SpareAmount = sale.ASESpareAmount;
            sale.VMCont.Conts = (List<SC_ASEContribution>)Session["conts"];
            return View(sale);
        }

        #region ASE Contribution Single Line
        [HttpPost]
        [CustomAuthorize(Roles = "Admin, ACCont")]
        public ActionResult SaveCont(VMCont VMCont, int Index = 0)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            VMCont.Conts = (List<SC_ASEContribution>)Session["conts"];
            try
            {
                if (ModelState.IsValid)
                {
                    if (VMCont.Cont.SL == 0)
                    {

                        VMCont.AddCont();

                    }
                    else
                    {
                        VMCont.UpdateCont();

                    }
                }
            }
            catch (Exception err)
            {
                ViewBag.ContError = err.Message.ToString();
            }
            //-----------assing ASE object---------------
            foreach (var cont in VMCont.Conts)
            {
                cont.ASE = setup.Get(cont.ASEID);
            }
            Session["conts"] = VMCont.Conts;
            ViewBag.ASE = setup.GetASE(UI);
            return PartialView("_Contribution", VMCont);
        }
        [CustomAuthorize(Roles = "Admin, ACCont")]
        public ActionResult EditCont(int Index)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            var VMCont = new VMCont();
            VMCont.Conts = (List<SC_ASEContribution>)Session["conts"];
            //-----------assing ASE object---------------
            foreach (var cont in VMCont.Conts)
            {
                cont.ASE = setup.Get(cont.ASEID);
            }
            VMCont.EditCont(Index);
            ViewBag.ASE = setup.GetASE(UI);
            return PartialView("_Contribution", VMCont);
        }
        [CustomAuthorize(Roles = "Admin, ACCont")]
        public ActionResult DeleteCont(int Index)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            var VMCont = new VMCont();
            VMCont.Conts = (List<SC_ASEContribution>)Session["conts"];
            //-----------assing ASE object---------------
            foreach (var cont in VMCont.Conts)
            {
                cont.ASE = setup.Get(cont.ASEID);
            }
            VMCont.DeleteCont(Index);
            Session["conts"] = VMCont.Conts;
            ViewBag.ASE = setup.GetASE(UI);
            return PartialView("_Contribution", VMCont);
        }

        #endregion
        #endregion

        #region   Approved ASE Commision

        // For Filter
        public ActionResult ApprovedASECom(string month , string  year , string ASEID)
        {

            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            DateTime firstdate = DateTime.Now, lastdate = DateTime.Now;
            string duration = "0";
            
            if (ASEID == "")
                ASEID = "0";

            if (month == "")
                month = "0";
            if (year == "")
                year = "0";
            //ViewBag.ASE = user.GetASE(UI);

            ViewBag.ASE = user.GetSelectListByUser(UI.User.UserID);

            List<SelectListItem> months = new List<SelectListItem>()
            {
            new SelectListItem{ Text="Please Select Month", Value = "0" },
            new SelectListItem{ Text="January", Value = "1" },
            new SelectListItem{ Text="February", Value = "2" },
            new SelectListItem{ Text="March", Value = "3" },
            new SelectListItem{ Text="April", Value = "4" },
            new SelectListItem{ Text="May", Value = "5" },
            new SelectListItem{ Text="June", Value = "6" },
            new SelectListItem{ Text="July", Value = "7" },
            new SelectListItem{ Text="August", Value = "8" },
            new SelectListItem{ Text="September", Value = "9" },
            new SelectListItem{ Text="October", Value = "10" },
             new SelectListItem{ Text="November", Value = "11" },
              new SelectListItem{ Text="December", Value = "12" },
            };

            ViewData["months"] = months;



            var calenderData = salecom.GetCalenderData(Convert.ToInt32(year), Convert.ToInt32(month)).FirstOrDefault();

            if (calenderData != null)
            {
                firstdate = calenderData.startDate;
                lastdate = calenderData.endDate;
                duration = calenderData.duration.ToString();
            }



            //if (Start == null || End == null)
            //{
            //    firstdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            //    lastdate = firstdate.AddMonths(1).AddDays(-1);
            //    ViewBag.startDate = firstdate.ToString("MM/dd/yyyy");
            //    ViewBag.endDate = lastdate.ToString("MM/dd/yyyy");
            //}

            //else
            //{
            //    firstdate = Convert.ToDateTime(Start);
            //    lastdate = Convert.ToDateTime(End);
            //    ViewBag.startDate = firstdate.ToString("MM/dd/yyyy");
            //    ViewBag.endDate = lastdate.ToString("MM/dd/yyyy");

            //}


            var data = salecom.GetASEData(UI, firstdate, lastdate, Convert.ToInt32(ASEID), duration);




            var calculationResult = salecom.GetASECalculation(data, firstdate, lastdate, Convert.ToInt32(ASEID));


            ViewBag.UserDetails = salecom.GetUserDetails(Convert.ToInt32(ASEID));

            ViewBag.calculationResult = calculationResult;

            ViewBag.year = year;
            ViewBag.duration = duration;

            ViewBag.startDate = firstdate.ToString("MM/dd/yyyy");
            ViewBag.endDate = lastdate.ToString("MM/dd/yyyy");

            ViewBag.ASEID = ASEID;
            ViewBag.Info = TempData["Info"];
            ViewBag.Error = TempData["Error"];


            var IsApproved = salecom.CheckApproved(UI, firstdate, lastdate, Convert.ToInt32(ASEID));

            if (IsApproved == 1)
            {
                ViewBag.ApprovedLog = 1;
            }
            else
            {
                ViewBag.ApprovedLog = 0;
            }

            return View(data);
        }


        // For Approved
        public ActionResult ApprovedASEReport(SC_ASEApprovedData calculation)
        {

            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            int userType = (int)UI.User.userType;
            calculation.UserID = UI.User.UserID;
            calculation.HostName = Request.ServerVariables["REMOTE_HOST"];
            calculation.EntryDateTime = DateTime.Now;
            calculation.status = userType;
            calculation.month = calculation.endDate.Month;
            calculation.year = calculation.endDate.Year;

            salecom.ApprovedASEReport(calculation);


            var data = salecom.GetASEData(UI, calculation.startDate, calculation.endDate, Convert.ToInt32(calculation.ASEID), calculation.duration.ToString());

            try
            {
                salecom.Save();
                TempData["Info"] = "Approved Succssfully";
                salecom.UpdateCollectionStatus(data, calculation.startDate, calculation.endDate, Convert.ToInt32(calculation.ASEID), userType);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return Redirect(HttpContext.Request.Headers["Referer"]);
            // return RedirectToAction("ApprovedASECom");
        }


        //Approved Report Generate 

        public ActionResult ApprovedReport(string Start, string End, string ASEID)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            DateTime firstdate, lastdate;

            //ViewBag.ASE = user.GetASE(UI);
            if (ASEID == "")
                ASEID = "0";

            ViewBag.ASE = user.GetSelectListByUser(UI.User.UserID);
            if (Start == null || End == null)
            {
                firstdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                lastdate = firstdate.AddMonths(1).AddDays(-1);
                ViewBag.startDate = firstdate.ToString("MM/dd/yyyy");
                ViewBag.endDate = lastdate.ToString("MM/dd/yyyy");
            }

            else
            {
                firstdate = Convert.ToDateTime(Start);
                lastdate = Convert.ToDateTime(End);
                ViewBag.startDate = firstdate.ToString("MM/dd/yyyy");
                ViewBag.endDate = lastdate.ToString("MM/dd/yyyy");

            }


            var data = salecom.GetASEApprovedData(UI, firstdate, lastdate, Convert.ToInt32(ASEID)).ToList();



            return View(data);
        }


        public ActionResult RejectInvoice(string paymentDoc, string invoiceDoc, string saleType, string customername, string ASEID, string Start, string End)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            var data = salecom.RejectInvoice(UI, paymentDoc, invoiceDoc, saleType, customername, Convert.ToInt32(ASEID), Convert.ToDateTime(Start), Convert.ToDateTime(End));

            try
            {
                salecom.Save();
            }
            catch (Exception ex)
            {

            }


            return Json(data);
        }


        #endregion



    }
}