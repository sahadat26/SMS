using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using SMS.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace SMS.Controllers
{
    public class ConditionMatrixController : Controller
    {
        //
        // GET: /Condition Matrix/
        private IConditionMatrixRepository condition { get; set; }
        private IUserRepository setup { get; set; }
        public ConditionMatrixController(IConditionMatrixRepository _condition, IUserRepository _setup)
        {
            condition = _condition;
            setup = _setup;
        }

        #region Condition Matrix CRUD

        [CustomAuthorize(Roles = "Admin, Condition")]
        public ActionResult Index(string q = "")
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

            if (q.Length > 0)
            {
                var SearchResult = condition.GetAll(UI.BusinessUnit).Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderBy(e => e.ID);
                ViewBag.SearchText = q;

                return View(SearchResult);
            }

            return View(condition.GetAll(UI.BusinessUnit).OrderBy(e => e.ID));
        }

        [CustomAuthorize(Roles = "Admin, Condition")]
        public ActionResult Create()
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
            return View(new SC_ConditionMatrix { BU = UI.BusinessUnit });
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Condition")]
        public ActionResult Create(SC_ConditionMatrix Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {

                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                condition.Add(Obj);
                condition.Save();
                TempData["Info"] = "New Condition Created";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("Create");
        }
        [CustomAuthorize(Roles = "Admin, Condition")]
        public ActionResult Edit(int CID)
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
            var oldone = condition.Get(CID);
            return View(oldone);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Condition")]
        public ActionResult Edit(SC_ConditionMatrix Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {

                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                condition.Update(Obj);
                condition.Save();
                TempData["Info"] = "Update has been made";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("Edit", new { CID = Obj.ID });
        }

        #endregion
        #region ASE Target

        [CustomAuthorize(Roles = "Admin,ASEMaster")]
        public ActionResult IndexAT(string q = "")
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

            if (q.Length > 0)
            {
                var SearchResult = condition.GetAllAT(UI.BusinessUnit).Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderBy(e => e.ID);
                ViewBag.SearchText = q;

                return View(SearchResult);
            }

            return View(condition.GetAllAT(UI.BusinessUnit).OrderBy(e => e.ID));
        }

        [CustomAuthorize(Roles = "Admin,ASEMaster")]
        public ActionResult CreateAT()
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
            ViewBag.ASE = setup.GetASE(UI);
            var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return View(new SC_ASETarget { BU = UI.BusinessUnit, StartDate = firstDayOfMonth, EndDate = lastDayOfMonth });
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, ASEMaster")]
        public ActionResult CreateAT(SC_ASETarget Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {

                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                condition.AddAT(Obj);
                condition.Save();
                TempData["Info"] = "New Target Created";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            ViewBag.ASE = setup.GetASE(UI);
            return RedirectToAction("CreateAt");
        }
        [CustomAuthorize(Roles = "Admin, ASEMaster")]
        public ActionResult EditAT(int ATID)
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
            var oldone = condition.GetAT(ATID);
            ViewBag.ASE = setup.GetASE(UI);
            return View(oldone);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, ASEMaster")]
        public ActionResult EditAT(SC_ASETarget Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {

                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                condition.UpdateAT(Obj);
                condition.Save();
                TempData["Info"] = "Update has been made";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            ViewBag.ASE = setup.GetASE(UI);
            return RedirectToAction("EditAT", new { ATID = Obj.ID });
        }

        [CustomAuthorize(Roles = "Admin, ASEMaster")]
        public ActionResult SetCalender()
        {
            var data = condition.ListofYear();
            return View(data);
        }
        [CustomAuthorize(Roles = "Admin, ASEMaster")]
        [HttpPost]
        public ActionResult SetCalender(int year, int duration)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            setupCalender obj =new setupCalender();
            obj.year=year;
            obj.duration=duration;
            condition.CalenderAddUpdate(obj,UI);
            condition.Save();
            var data = condition.ListofYear();


            return View(data);
        }
        public ActionResult CalenderList(int year)
        {
            var data = condition.ListofYears(year);

            return View(data);
        }

        [CustomAuthorize(Roles = "Admin, ASEMaster")]
        public ActionResult Upload()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Admin, ASEMaster")]
        [HttpPost]
        public ActionResult Upload(FormCollection formCollection)
        {
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    var UI = UserInfo.GetUserInfo(User.Identity.Name);
                   
                    int rowCount = 0;
                    try
                    {
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            for (int i = 2; i <= noOfRow; i++)
                            {
                                SC_ASETarget Obj = new SC_ASETarget();
                                Obj.EntryDateTime = DateTime.Now;
                                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                                Obj.UserID = UI.User.UserID;

                                string userName = workSheet.Cells[i, 1].Value.ToString();
                                int targetType = Convert.ToInt32(workSheet.Cells[i, 2].Value);
                                int month = Convert.ToInt32(workSheet.Cells[i, 3].Value);
                                int year = Convert.ToInt32(workSheet.Cells[i, 4].Value);
                                DateTime startDate = Convert.ToDateTime(workSheet.Cells[i, 5].Value);
                                DateTime endDate = Convert.ToDateTime(workSheet.Cells[i, 6].Value);
                                decimal targetSalesAmt = Convert.ToDecimal(workSheet.Cells[i, 7].Value);
                                decimal targetSalesCollectionAmt = Convert.ToDecimal(workSheet.Cells[i, 8].Value);
                                decimal achivedPer = Convert.ToDecimal(workSheet.Cells[i, 9].Value);
                                int newContract = Convert.ToInt32(workSheet.Cells[i, 10].Value);
                                int renewContract = Convert.ToInt32(workSheet.Cells[i, 11].Value);

                                int ASEID = condition.GetASEID(userName);

                                if (ASEID != null)
                                {
                                    Obj.BU = EBusinessUnit.SnS;
                                    Obj.ASEUID = ASEID;
                                    Obj.TargetType = targetType == 1 ? ETargetType.Monthly : ETargetType.Yearly;
                                    if (month == 1)
                                        Obj.MonthYearName = EMonth.January;
                                    else if (month == 2)
                                        Obj.MonthYearName = EMonth.February;
                                    else if (month == 3)
                                        Obj.MonthYearName = EMonth.March;
                                    else if (month == 4)
                                        Obj.MonthYearName = EMonth.April;
                                    else if (month == 5)
                                        Obj.MonthYearName = EMonth.May;
                                    else if (month == 6)
                                        Obj.MonthYearName = EMonth.June;
                                    else if (month == 7)
                                        Obj.MonthYearName = EMonth.July;
                                    else if (month == 8)
                                        Obj.MonthYearName = EMonth.Auguest;
                                    else if (month == 9)
                                        Obj.MonthYearName = EMonth.September;
                                    else if (month == 10)
                                        Obj.MonthYearName = EMonth.October;
                                    else if (month == 11)
                                        Obj.MonthYearName = EMonth.November;
                                    else if (month == 12)
                                        Obj.MonthYearName = EMonth.December;
                                    Obj.Year = year;
                                    Obj.StartDate = startDate;
                                    Obj.EndDate = endDate;
                                    Obj.TargetAmount = targetSalesAmt;
                                    Obj.AchievedAmount = targetSalesCollectionAmt;
                                    Obj.AchievedPerc = achivedPer;
                                    Obj.NewContract = newContract;
                                    Obj.RenewContract = renewContract;

                                    condition.AddUpdateAT(Obj);
                                    rowCount++;

                                }

                                //usersList.Add(user);  
                            }
                            condition.Save();
                            ViewBag.Info = rowCount + " " + "New Target Created";
                        }
                    }
                    catch(Exception ex)
                    {
                        ViewBag.Error = ex.Message.ToString();
                    }
                    
                }
            }

            return View();
        }



        #endregion

    }
}