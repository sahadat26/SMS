using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.Entity;
using SMS.Interfaces;
using System.Web.Security;
using System.IO;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Requisition/

        private IProductRepository product { get; set; }
        private IUserRepository setup { get; set; }

        public ProductController(IProductRepository _product,IUserRepository _setup)
        {
            product = _product;
            setup = _setup;
        }

        #region CRUD Prodcut Category
        [CustomAuthorize(Roles = "Admin, Product")]
        public ActionResult ManageCategory()
        {
            ViewBag.Categories = product.GetCategoryAll().ToList();
            ViewBag.CatList = product.GetCategorySelectList();
            return PartialView(new ProductCategory());
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Product")]
        public ActionResult CategoryAddUpdate(ProductCategory Obj)
        {

            try
            {
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = SMS.Models.User.getUser(User.Identity.Name).UserID;
                if (Obj.ProductCategoryID == 0)
                {
                    product.AddCategory(Obj);
                }
                else
                {
                    product.UpdateCategory(Obj);
                }
                product.Save();

                var Categories = from row in product.GetCategoryAll()
                                 select new
                                 {
                                     ProductCategoryID=row.ProductCategoryID,
                                     CategoryName=row.CategoryName,
                                     Parent=row.ParentCategory.CategoryName
                                 };
                return Json(new { flag = "true", msg = "Category has been saved", listdata = Categories });
            }
            catch (Exception err)
            {
                return Json(new { flag = "false", msg = err.Message.ToString() });
            }

        }

        [CustomAuthorize(Roles = "Admin, Product")]
        public ActionResult CategoryEdit(int ID)
        {
            var oldone = product.GetCategory(ID);
            var obj = new
            {
                ProductCategoryID = oldone.ProductCategoryID,
                CategoryName = oldone.CategoryName,
                ParentId = oldone.ParentId
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        #endregion
        #region SAP Product
        [CustomAuthorize]
        public ActionResult SAPProductList(DateTime? Start, DateTime? End, string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            bool WithDate = (Request["WithDate"] ?? "") == "true,false" ? true : false;
            if (!Start.HasValue && !End.HasValue)
            {

                Start = new DateTime?(DateTime.Today.AddMonths(-3));
                End = new DateTime?(DateTime.Today.AddDays(1));

            }
            ViewBag.StartDate = Start.Value.ToString("MM/dd/yyyy");
            ViewBag.EndDate = End.Value.ToString("MM/dd/yyyy");
            ViewBag.SearchText = q;
            ViewBag.Chk = WithDate;
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
                if(WithDate)
                {
                    var SearchResult = product.GetSAPGenset()
                    .Where(c => c.DELIVERYDATE >= Start && c.DELIVERYDATE <= End)
                    .Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.DELIVERYDATE);

                    return View(SearchResult);
                }
                else
                {
                    var SearchResult = product.GetSAPGenset()
                    .Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.DELIVERYDATE);
                    return View(SearchResult);
                }
                
            }
            if(WithDate)
            {
                return View(product.GetSAPGenset()
                    .Where(c => c.DELIVERYDATE >= Start && c.DELIVERYDATE <= End)
                    .OrderByDescending(e => e.DELIVERYDATE));
            }

            return View(product.GetSAPGenset()
                    .OrderByDescending(e => e.DELIVERYDATE));
        }
        [CustomAuthorize]
        public ActionResult IndexPartial(DateTime? Start, DateTime? End, string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            bool WithDate = (Request["WithDate"] ?? "") == "true,false" ? true : false;
            if (!Start.HasValue && !End.HasValue)
            {

                Start = new DateTime?(DateTime.Today.AddMonths(-3));
                End = new DateTime?(DateTime.Today.AddDays(1));

            }
            ViewBag.StartDate = Start.Value.ToString("MM/dd/yyyy");
            ViewBag.EndDate = End.Value.ToString("MM/dd/yyyy");
            ViewBag.SearchText = q;
            ViewBag.Chk = WithDate;
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
                if (WithDate)
                {
                    var SearchResult = product.GetSAPGenset()
                    .Where(c => c.DELIVERYDATE >= Start && c.DELIVERYDATE <= End)
                    .Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.DELIVERYDATE);

                    return PartialView("_IndexPartial",SearchResult);
                }
                else
                {
                    var SearchResult = product.GetSAPGenset()
                    .Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.DELIVERYDATE);
                    return PartialView("_IndexPartial", SearchResult);
                }

            }
            if (WithDate)
            {
                return PartialView("_IndexPartial", product.GetSAPGenset()
                    .Where(c => c.DELIVERYDATE >= Start && c.DELIVERYDATE <= End)
                    .OrderByDescending(e => e.DELIVERYDATE));
            }

            return PartialView("_IndexPartial", product.GetSAPGenset()
                    .OrderByDescending(e => e.DELIVERYDATE));
            
        }
        [CustomAuthorize]
        public async Task<ActionResult> SyncSAPProduct(DateTime Start, DateTime End, string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                var orgs = setup.OrgAll(UI.BusinessUnit);
                UI.Host = Request.ServerVariables["REMOTE_HOST"];
                foreach (var org in orgs)
                {
                    var products = await product.SyncSAPProduct(org.ProductSalesOrg, Start, End,"");
                    var tobeadded = (from row in products.Where(c => c.DOCTYPE == "ZPCA" || c.DOCTYPE == "ZPCR")
                                     where !((from prow in product.GetSAPProdAll() select prow.DOCNO).Contains(row.DOCNO)
                                     && (from prow in product.GetSAPProdAll() select prow.ITEMNO).Contains(row.ITEMNO)
                                     )
                                     select row).ToList();
                    var grp = (from row in tobeadded
                               group row by new { row.DOCNO, row.ITEMNO }
                                   into g
                                   select new
                                       {
                                           DocNo = g.Key.DOCNO,
                                           ItemNo = g.Key.ITEMNO,
                                           Nos = g.Count(),
                                       }).OrderByDescending(c => c.Nos).ToList();
                    //foreach(var delitem in grp.Where(c=>c.Nos>1))
                    //{
                    //    tobeadded.RemoveAll(c => c.ENGINESERIAL == delitem.Serial);
                    //}
                    //grp = (from row in tobeadded
                    //           group row by new { row.DOCNO, row.ENGINESERIAL }
                    //               into g
                    //               select new
                    //               {
                    //                   DocNo = g.Key.DOCNO,
                    //                   Serial = g.Key.ENGINESERIAL,
                    //                   Nos = g.Count(),
                    //               }).OrderByDescending(c => c.Nos).ToList();
                    foreach (var pro in tobeadded)
                    {
                        var pgc = product.GetPGC(pro.GRP);
                        if(pgc==null)
                        {
                            pro.Brand = "None";
                        }
                        else
                        {
                            pro.Brand = pgc.Brand;
                        }
                        pro.CommissioningDate = pro.DELIVERYDATE;
                        pro.EQModel = "";
                        pro.EQSerial = "";
                        pro.Location = pro.ADDRESS;
                        pro.Rating = pro.MATERIALNAME;
                        product.AddSAPProduct(pro);
                    }
                    product.Save();


                    TempData["Info"] = "Date sucessfully synced";
                }

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
                TempData["Error"] = errMsg;
            }
            catch (Exception err)
            {
                TempData["Error"] = err.InnerException.Message;
            }
            return RedirectToAction("SAPProductList", new { Start = Start, End = End, q = q });
        }

        #endregion

        #region CRUD Product
        [CustomAuthorize(Roles = "Admin, Product")]
        public ActionResult Index()
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

            return View(product.GetProductAll(UI.BusinessUnit).OrderBy(a=>a.ProductName));
        }

        [CustomAuthorize(Roles = "Admin, Product")]
        public ActionResult AddProduct()
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
            ViewBag.Categories = product.GetCategorySelectList();
            return View(new Product {BusinessUnit=UI.BusinessUnit });
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Product")]
        public ActionResult AddProduct(Product Obj, HttpPostedFileBase FileToUpload)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            
            try
            {
                #region Upload Validation
                if (FileToUpload!=null)
                {

                    if (FileToUpload.ContentLength > 1048576)
                    {
                        TempData["Error"] = "you can upload max 1mb file!";
                    }
                    else if (!((FileToUpload.ContentType == "image/png")
                        || (FileToUpload.ContentType == "image/jpeg")
                        || (FileToUpload.ContentType == "image/gif")
                        ))
                    {
                        TempData["Error"] = "Given file not a valid Image";
                    }
                    else
                    {
                        
                        string destination = Server.MapPath("~/Upload/Image/");
                        string ext = Path.GetExtension(FileToUpload.FileName);
                        string NewFileName = Obj.BusinessUnit+"_" + Obj.ProductName.Replace(' ', '_')+ext;
                        FileToUpload.SaveAs(destination + NewFileName);
                        Obj.ImageURL = NewFileName;
                    }
                    
                }
                #endregion
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                product.AddProduct(Obj);
                product.Save();
                return RedirectToAction("AddProductProperty", new { ProductID = Obj.ProductID });
            }
            catch(Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            
            return RedirectToAction("AddProduct");
        }
        [CustomAuthorize(Roles = "Admin, Product")]
        public ActionResult EditProduct(int ProductID=0)
        {
            if(ProductID==0)
            {
                throw new Exception("No Product Selected");
            }
            var oldone = product.GetProduct(ProductID);
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Info = TempData["Info"];
            }
            ViewBag.Categories = product.GetCategorySelectList();
            return View(oldone);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Product")]
        public ActionResult EditProduct(Product Obj, HttpPostedFileBase FileToUpload)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {
                #region Upload Validation
                if (FileToUpload != null)
                {

                    if (FileToUpload.ContentLength > 1048576)
                    {
                        TempData["Error"] = "you can upload max 1mb file!";
                    }
                    else if (!((FileToUpload.ContentType == "image/png")
                        || (FileToUpload.ContentType == "image/jpeg")
                        || (FileToUpload.ContentType == "image/gif")
                        ))
                    {
                        TempData["Error"] = "Given file not a valid Image";
                    }
                    else
                    {

                        string destination = Server.MapPath("~/Upload/Image/");
                        string ext = Path.GetExtension(FileToUpload.FileName);
                        string NewFileName = Obj.BusinessUnit + "_" + Obj.ProductName.Replace(' ', '_') + ext;
                        FileToUpload.SaveAs(destination + NewFileName);
                        Obj.ImageURL = NewFileName;
                    }

                }
                #endregion
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                product.UpdateProduct(Obj);
                product.Save();
                return RedirectToAction("AddProductProperty", new { ProductID = Obj.ProductID });
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("EditProduct", new { ProductID=Obj.ProductID});
        }
        #endregion
        #region Product Property
        [CustomAuthorize(Roles = "Admin, Product")]
        public ActionResult AddProductProperty(int ProductID=0)
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Info = TempData["Info"];
            }
            if (ProductID == 0)
                throw new Exception("No Product Selected!");
            ViewBag.Item = product.GetProduct(ProductID);
            ViewBag.Properties = product.GetPropertySelectlist();
            return View(new ProductProperty { ProductID=ProductID});
        }
        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Product")]
        public ActionResult SaveProductProperty(ProductProperty Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            string msg = "";
            try
            {
                if(Obj.ProductPropertyID==0)
                {
                    product.AddProductProperty(Obj, Obj.ProductID);
                    msg = "New Property Added Successfuly";
                }
                else
                {
                    product.ModifyProductProperty(Obj);
                    msg = "Property updated Successfuly";
                }
                product.Save();
                TempData["Info"]=msg;
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("AddProductProperty", new { ProductID = Obj.ProductID });
        }
        [CustomAuthorize(Roles = "Admin, Product")]
        public ActionResult EditProductProperty(int ProductPropertyID)
        {
            if (ProductPropertyID == 0)
                throw new Exception("No Property found!");
            var oldone = product.GetProductProperty(ProductPropertyID);
            ViewBag.Item = product.GetProduct(oldone.ProductID);
            ViewBag.Properties = product.GetPropertySelectlist();
            return View("AddProductProperty", oldone);
        }
        #endregion
        
        #region CRUD Property
        [CustomAuthorize(Roles = "Admin, Item")]
        public ActionResult ManageProperty()
        {
            ViewBag.Properties = product.GetPropertyAll().ToList();
            return PartialView(new Property());
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Item")]
        public ActionResult PropertyAddUpdate(Property Obj)
        {

            try
            {
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = SMS.Models.User.getUser(User.Identity.Name).UserID;
                if (Obj.PropertyID == 0)
                {
                    product.AddProperty(Obj);
                }
                else
                {
                    product.UpdateProperty(Obj);
                }
                product.Save();

                var Properties = product.GetPropertyAll();
                return Json(new { flag = "true", msg = "Property has been saved", listdata = Properties });
            }
            catch (Exception err)
            {
                return Json(new { flag = "false", msg = err.Message.ToString() });
            }

        }

        [CustomAuthorize(Roles = "Admin, Item")]
        public ActionResult PropertyEdit(int ID)
        {
            var oldone = product.GetProperty(ID);
            return Json(oldone, JsonRequestBehavior.AllowGet);
        }


        #endregion

    }
}
