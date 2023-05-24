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
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace SMS.Repositories
{
    public class SalesCommissionRepository : ISalesCommissionRepository
    {
        #region Properties
        private SMSContext db { get; set; }
        private UserRepository user { get; set; }
        public SalesCommissionRepository(SMSContext _db, UserRepository _user)
        {
            db = _db;
            user = _user;
        }
        public SalesCommissionRepository()
            : this(new SMSContext(), new UserRepository())
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

        #region Sales
        public SC_Sales GetSale(string Invoice_Doc)
        {
            return db.SC_Sales.Find(Invoice_Doc);
        }

        public IEnumerable<SC_Sales> GetAllSales(UserInfo UI)
        {
            if (UI.User.BusinessUnit == EBusinessUnit.ALL)
                return db.SC_Sales;
            var users = user.GetAllUser(UI.User.UserID, new List<int>());
            if (UI.User.IsASE == true)
            {
                var list3 = from row in db.SC_ASEContribution
                            join prow in users on row.ASEID equals prow
                            select row.Sale;
                return list3;
            }
            var list = (from row in db.SC_Sales
                        join prow in users on row.SMUID equals prow
                        select row).ToList();
            var list2 = (from row in db.SC_Sales
                         join prow in users on row.SPUID equals prow
                         select row).ToList();
            var list4 = (from row in list2
                         where !(from prow in list select prow.Invoice_Doc).Contains(row.Invoice_Doc)
                         select row).ToList();
            list.AddRange(list4);
            return list;
        }

        public IEnumerable<SC_Sales> GetAllSales()
        {
            return db.SC_Sales;
        }

        public void AddSale(SC_Sales obj, UserInfo UI)
        {
            obj.BU = UI.BusinessUnit;
            obj.EntryDateTime = DateTime.Now;
            obj.HostName = UI.Host;
            obj.SMUID = (user.GetUserBySAPCode(obj.SM_ID) ?? new User()).UserID;
            obj.SPUID = (user.GetUserBySAPCode(obj.SP_ID) ?? new User()).UserID;
            obj.UserID = UI.User.UserID;
            if ((obj.ASE_ID ?? "") != "")
            {
                obj.ASE_Portion = obj.SpareAmount;
            }
            db.SC_Sales.Add(obj);
        }

        public async Task<IEnumerable<SC_Sales>> SyncSales(string SalesOrg, DateTime fromdt, DateTime todt, string Div, string flag = "sale")
        {
            IEnumerable<SC_Sales> sales = new List<SC_Sales>();
            string uri = Common.BaseUrl + "get_sales?sap-client=" + Common.Client + "&salesorg=" + SalesOrg + "&fromdt=" + fromdt.ToString("yyyyMMdd") +
                "&todt=" + todt.ToString("yyyyMMdd") + "&div=" + Div + "&flag=" + flag;

            var Credential = new NetworkCredential(Common.UserID, Common.Password);
            var Handler = new HttpClientHandler { Credentials = Credential };
            HttpClient _client = new HttpClient(Handler);


            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                sales = JsonConvert.DeserializeObject<IEnumerable<SC_Sales>>(content);
            }
            return sales;
        }

        public bool IsUserPermitted(string Invoice, UserInfo UI)
        {
            bool flag = false;
            var sale = GetAllSales(UI).Where(s => s.Invoice_Doc == Invoice).FirstOrDefault();
            if (sale != null)
            {
                return true;
            }
            return flag;
        }

        public void ASEPortionUpdate(VMASEPart obj)
        {
            if (obj.ASEPartAmount > obj.SpareAmount)
            {
                throw new Exception("ASE portion cannot be greater than Total Spare Amount");
            }
            else
            {
                var sale = GetSale(obj.InvoiceNo);
                if (sale != null)
                {
                    sale.ASE_Portion = obj.ASEPartAmount;
                    var oldasepart = sale.Contributions.Select(c => c.ContrAmount).DefaultIfEmpty(0).Sum();
                    foreach (var cont in sale.Contributions)
                    {
                        if (oldasepart > 0)
                        {
                            var perc = (cont.ContrAmount * 100) / oldasepart;
                            var _cont = GetAC(cont.ID);
                            _cont.ContrPerc = perc;
                            _cont.ContrAmount = (sale.ASE_Portion * perc) / 100;
                        }

                    }
                }
            }
        }
        #endregion
        #region Collection
        public SC_Collection GetCollection(int ID)
        {
            return db.SC_Collection.Find(ID);
        }

        public IEnumerable<SC_Collection> GetAllCollection(UserInfo UI)
        {


            if (UI.User.BusinessUnit == EBusinessUnit.ALL)
                return db.SC_Collection;


            var users = user.GetAllUser(UI.User.UserID, new List<int>());
            //if (UI.User.IsASE == true)
            //{
            //    var list3 = from row in db.SC_ASEContribution
            //                join prow in users on row.ASEID equals prow
            //                select row.Sale;
            //    var list4 = (from row in db.SC_Collection
            //                 join prow in list3 on row.INVOICE_DOC equals prow.Invoice_Doc
            //                 select row).ToList();
            //    return list4;
            //}
            //var list = (from row in db.SC_Collection
            //            join prow in users on row.SMUID equals prow
            //            select row).ToList();
            //var list2 = (from row in db.SC_Collection
            //             join prow in users on row.SPUID equals prow
            //             select row).ToList();
            //var list5 = (from row in list2
            //             where !(from prow in list select prow.ID).Contains(row.ID)
            //             select row).ToList();


            if(UI.User.IsASE==true)
            {
                var list = (from row in db.SC_Collection
                            join contribution in db.SC_ASEContribution
                            on row.INVOICE_DOC equals contribution.Invoice_Doc
                            where row.SALE_TYPE == "SPARE"
                            && users.Contains(contribution.ASEID)
                            select row).ToList();


                var listAssignTDSVDS = (from row in db.SC_Collection
                                        join contribution in db.SC_ASEContribution
                                        on row.INVOICE_DOC equals contribution.Invoice_Doc
                                        where row.SALE_TYPE != "SPARE"
                                        && users.Contains(row.ASEUID)
                                        select row).ToList();

                list.AddRange(listAssignTDSVDS);

                return list;
            }
            else
            {

                //var list = (from row in db.SC_Collection
                //            join contribution in db.SC_ASEContribution
                //            on row.INVOICE_DOC equals contribution.Invoice_Doc
                //            where row.SALE_TYPE == "SPARE"
                //            && users.Contains(contribution.ASEID)
                //            select row).ToList();

                //var listAssignTDSVDS = (from row in db.SC_Collection
                //                        join contribution in db.SC_ASEContribution
                //                        on row.INVOICE_DOC equals contribution.Invoice_Doc
                //                        where row.SALE_TYPE != "SPARE"
                //                        && users.Contains(row.ASEUID)
                //                        select row).ToList();


                var listOthers = (from row in db.SC_Collection

                                  //where row.SALE_TYPE != "SPARE"
                                  //&& row.ASEUID == 0
                                  select row).ToList();

              

                //list.AddRange(listAssignTDSVDS);

                //list.AddRange(listOthers);

                return listOthers;
            }



        }
        public IEnumerable<SC_Collection> GetAllCollection()
        {
            return db.SC_Collection;
        }

        public void AddCollection(SC_Collection obj, UserInfo UI)
        {
            obj.IsLastPayment = false;
            obj.BU = UI.BusinessUnit;
            obj.EntryDateTime = DateTime.Now;
            obj.HostName = UI.Host;
            obj.SMUID = (user.GetUserBySAPCode(obj.SM_ID) ?? new User()).UserID;
            obj.SPUID = (user.GetUserBySAPCode(obj.SP_ID) ?? new User()).UserID;
            obj.UserID = UI.User.UserID;
            db.SC_Collection.Add(obj);
        }

        public void SetLastPayment(int ID)
        {
            var pmt = GetCollection(ID);
            if (pmt != null)
            {
                if (pmt.IsLastPayment)
                {
                    pmt.IsLastPayment = false;
                }
                else
                {
                    pmt.IsLastPayment = true;
                }
            }
        }

        public void SetColASE(VMColASE obj,UserInfo UI)
        {

            var pmt = GetCollection(obj.ID);
            if (pmt != null)
            {
                pmt.ASEUID = obj.ASEUID;
                pmt.assignBy = UI.User.UserID;
                pmt.assignDisplayName = UI.User.UserName + " : " + "" + UI.User.FullName;
            }
        }

        public async Task<IEnumerable<SC_Collection>> SyncCollection(string SalesOrg, DateTime fromdt, DateTime todt, string Div, string flag = "payment")
        {
            IEnumerable<SC_Collection> collections = new List<SC_Collection>();
            string uri = Common.BaseUrl + "get_sales?sap-client=" + Common.Client + "&salesorg=" + SalesOrg + "&fromdt=" + fromdt.ToString("yyyyMMdd") +
                "&todt=" + todt.ToString("yyyyMMdd") + "&div=" + Div + "&flag=" + flag;

            var Credential = new NetworkCredential(Common.UserID, Common.Password);
            var Handler = new HttpClientHandler { Credentials = Credential };
            HttpClient _client = new HttpClient(Handler);

            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                collections = JsonConvert.DeserializeObject<IEnumerable<SC_Collection>>(content);
            }
            return collections;
        }
        #endregion

        #region ASE Contribution
        public SC_ASEContribution GetAC(int ID)
        {
            return db.SC_ASEContribution.Find(ID);
        }

        public IEnumerable<SC_ASEContribution> GetACBySales(string Invoice_Doc)
        {
            return db.SC_ASEContribution.Where(m => m.Invoice_Doc == Invoice_Doc);
        }

        public IEnumerable<SC_ASEContribution> GetAllAC(UserInfo UI)
        {
            if (UI.BusinessUnit == EBusinessUnit.ALL)
                return db.SC_ASEContribution;
            var users = user.GetAllUser(UI.User.UserID, new List<int>());
            if (UI.User.IsASE == true)
            {
                var list3 = from row in db.SC_ASEContribution
                            join prow in users on row.ASEID equals prow
                            select row;
                return list3;
            }
            var list = (from row in db.SC_Sales
                        join prow in users on row.SMUID equals prow
                        select row).ToList();
            var list2 = (from row in db.SC_Sales
                         join prow in users on row.SPUID equals prow
                         select row).ToList();
            list.AddRange(list2);
            var list4 = from row in db.SC_ASEContribution
                        join prow in list on row.Invoice_Doc equals prow.Invoice_Doc
                        select row;

            return list4;
        }

        public IEnumerable<SC_ASEContribution> GetAllAC()
        {
            return db.SC_ASEContribution;
        }

        public void AddAC(IList<SC_ASEContribution> obj, UserInfo UI)
        {

            foreach (var cont in obj)
            {
                if (cont.ASEID == 0)
                {
                    throw new Exception("No ASE Selected for: " + cont.Invoice_Doc);
                }
                else
                {
                    cont.BU = UI.BusinessUnit;
                    cont.EntryDateTime = DateTime.Now;
                    cont.HostName = UI.Host;
                    cont.UserID = UI.User.UserID;
                    cont.ContrPerc = 100;
                    db.SC_ASEContribution.Add(cont);
                }
            }
        }

        public void UpdateAC(IList<SC_ASEContribution> obj, UserInfo UI)
        {
            var invoicedoc = obj.FirstOrDefault().Invoice_Doc;
            var sale = GetSale(invoicedoc);
            if (obj.Sum(m => m.ContrAmount) != sale.ASE_Portion)
            {
                throw new Exception("Contribution must be equal to ASE Portion: " + sale.ASE_Portion + " for: " + invoicedoc);
            }
            else
            {
                var tobeadded = obj.Where(c => c.ID == 0).ToList();
                var tobeupdated = obj.Where(c => c.ID != 0).ToList();
                var tobedeleted = (from row in db.SC_ASEContribution.Where(c => c.Invoice_Doc == invoicedoc).ToList()
                                   where !(from prow in obj.Where(c => c.ID != 0).ToList() select prow.ID)
                                   .Contains(row.ID)
                                   select row).ToList();

                foreach (var add in tobeadded)
                {
                    if (add.ASEID == 0)
                    {
                        throw new Exception("No ASE Selected for: " + add.Invoice_Doc);
                    }
                    else
                    {
                        var cont = new SC_ASEContribution();
                        cont.BU = UI.BusinessUnit;
                        cont.EntryDateTime = DateTime.Now;
                        cont.HostName = UI.Host;
                        cont.UserID = UI.User.UserID;
                        cont.ASEID = add.ASEID;
                        cont.ContrAmount = add.ContrAmount;
                        cont.SL = add.SL;
                        cont.Invoice_Doc = invoicedoc;
                        //-----------------by pass validation---------
                        cont.SpareAmount = cont.ContrAmount;
                        cont.ContrPerc = cont.DisConrPerc;
                        db.SC_ASEContribution.Add(cont);
                    }
                }
                foreach (var upd in tobeupdated)
                {
                    if (upd.ASEID == 0)
                    {
                        throw new Exception("No ASE Selected for: " + upd.Invoice_Doc);
                    }
                    else
                    {
                        var cont = GetAC(upd.ID);
                        cont.ASEID = upd.ASEID;
                        cont.ContrAmount = upd.ContrAmount;
                        //-----------------by pass validation---------
                        cont.SpareAmount = cont.ContrAmount;
                        cont.ContrPerc = cont.DisConrPerc;
                    }
                }

                tobedeleted.ForEach(d => db.SC_ASEContribution.Remove(d));
            }
        }


        #endregion


        public void Save()
        {
            db.SaveChanges();
        }


        public IEnumerable<SC_ASEReport> GetASEData(UserInfo UI, DateTime Start, DateTime End, int ASEID, string duration)
        {
            List<SC_ASEReport> aseReports = new List<SC_ASEReport>();
            List<SC_ASEReportTDSVDS> aseTDSVDS = new List<SC_ASEReportTDSVDS>();

            var userType = UI.User.userType;



            int durationPeriod = 0;

            if (duration != null)
            {
                durationPeriod = Convert.ToInt32(duration);
            }

            // dynamic query = null;
            if (UI.User.BusinessUnit == EBusinessUnit.SnS && userType != null)
            {
                if (ASEID == 0)
                    ASEID = -1;


                var users = user.GetAllUser(UI.User.UserID, new List<int>());

                var list3 = from row in db.SC_ASEContribution
                            join prow in users on row.ASEID equals prow
                            where row.ASEID == ASEID
                            select row.Sale;

                var spareSales = (from row in db.SC_Sales
                                      join contribution in list3 on row.Invoice_Doc
                                      equals contribution.Invoice_Doc
                                      where row.Invoice_Date >= Start && row.Invoice_Date <= End

                                      select contribution).ToList();

                decimal TotspareSales = 0;
                foreach(var item in spareSales)
                {
                    TotspareSales += item.TotalSpareContribution;
                }

           
              //  aseReport.SpareSalesAmt=spareCollection.Sum(x=>x.co)



                var listSpare = (int)userType == 1 ? ((from row in db.SC_Collection
                                                       join prow in list3 on row.INVOICE_DOC equals prow.Invoice_Doc
                                                       where
                                                       row.PAYMENT_DATE >= Start && row.PAYMENT_DATE <= End
                                                       && row.SALE_TYPE == "SPARE"
                                                       && row.isApproved != true
                                                       select row.Sale).ToList().Distinct())
                                 :

                                 ((from row in db.SC_Collection
                                   join prow in list3 on row.INVOICE_DOC equals prow.Invoice_Doc
                                   where
                                   row.PAYMENT_DATE >= Start && row.PAYMENT_DATE <= End
                                   && row.SALE_TYPE == "SPARE"
                                   && row.status == (int)userType - 1
                                   select row.Sale).ToList().Distinct())

                                 ;



                var listService = (int)userType == 1 ? ((from rowCollection in db.SC_Collection
                                                         join rowSale in db.SC_Sales on rowCollection.INVOICE_DOC equals rowSale.Invoice_Doc
                                                         where rowCollection.PAYMENT_DATE >= Start && rowCollection.PAYMENT_DATE <= End
                                                                && rowCollection.SALE_TYPE != "TDS"
                                                                && rowCollection.SALE_TYPE != "VDS"
                                                           && rowCollection.ASEUID == ASEID
                                                           && rowCollection.isApproved != true
                                                         select rowCollection).ToList().Distinct())
                                   :
                                   ((from rowCollection in db.SC_Collection
                                     join rowSale in db.SC_Sales on rowCollection.INVOICE_DOC equals rowSale.Invoice_Doc
                                     where rowCollection.PAYMENT_DATE >= Start && rowCollection.PAYMENT_DATE <= End
                                            && rowCollection.SALE_TYPE != "TDS"
                                            && rowCollection.SALE_TYPE != "VDS"
                                       && rowCollection.ASEUID == ASEID
                                       && rowCollection.status == (int)userType - 1
                                     select rowCollection).ToList().Distinct())
                                   ;



                var listTDS = (int)userType == 1 ? (db.SC_Collection.Where(x => x.PAYMENT_DATE >= Start && x.PAYMENT_DATE <= End && x.SALE_TYPE == "TDS" && x.ASEUID == ASEID && x.isApproved != true).GroupBy(x => x.CUSTOMER_NAME)
                                               .Select(a => new { amount = a.Sum(b => b.AMOUNT), customerName = a.Key }).ToList())
                                               :
                                               (db.SC_Collection.Where(x => x.PAYMENT_DATE >= Start && x.PAYMENT_DATE <= End && x.SALE_TYPE == "TDS" && x.ASEUID == ASEID && x.status == (int)userType - 1).GroupBy(x => x.CUSTOMER_NAME)
                                               .Select(a => new { amount = a.Sum(b => b.AMOUNT), customerName = a.Key }).ToList())
                                             ;

                var listVDS = (int)userType == 1 ? db.SC_Collection.Where(x => x.PAYMENT_DATE >= Start && x.PAYMENT_DATE <= End && x.SALE_TYPE == "VDS" && x.ASEUID == ASEID && x.isApproved != true).GroupBy(x => x.CUSTOMER_NAME)
                                               .Select(a => new { amount = a.Sum(b => b.AMOUNT), customerName = a.Key }).ToList()
                                               :
                                               db.SC_Collection.Where(x => x.PAYMENT_DATE >= Start && x.PAYMENT_DATE <= End && x.SALE_TYPE == "VDS" && x.ASEUID == ASEID && x.status == (int)userType - 1).GroupBy(x => x.CUSTOMER_NAME)
                                               .Select(a => new { amount = a.Sum(b => b.AMOUNT), customerName = a.Key }).ToList()
                                           ;




                foreach (var item in listSpare)
                {

                    DateTime CollectionDate = item.Invoice_Date;
                    foreach (var itemDetail in item.Collections)
                    {
                        CollectionDate = itemDetail.PAYMENT_DATE;
                    }

                    var aseReport = new SC_ASEReport();
                   
                    aseReport.InvoiceDate = item.Invoice_Date;
                    aseReport.Invoice_Doc = item.Invoice_Doc;
                    aseReport.CollectionDate = CollectionDate;
                    aseReport.Customer_Name = item.Customer_Name;
                    aseReport.ASEDisplayName = item.ASEDisplay;
                    aseReport.Revenue = item.Revenue;
                    aseReport.ASE_Portion = item.ASE_Portion;
                    aseReport.Collections = item.TotalCollectionSpare;
                    aseReport.SaleAmt = Math.Round(item.SaleAmount, 0);
                    aseReport.SpareSalesAmt = Math.Round(TotspareSales, 0); 
                    aseReport.Duration = (CollectionDate - item.Invoice_Date).Days;

                    aseReport.SaleType = item.Sale_Type;
                    aseReport.lubeQty = item.Lube_Qty;
                    aseReport.Claimable_ASE_Collection = Math.Round((item.ASE_Portion * item.TotalCollectionSpare / item.SaleAmount), 0);
                    if (aseReport.Duration <= durationPeriod)
                    {
                        if (aseReport.SaleAmt <= aseReport.Collections)
                        {
                            aseReport.Eligible = "YES";
                        }
                        else
                        {
                            if (item.IslastPayment == true)
                            {
                                aseReport.Eligible = "YES";
                            }
                            else
                            {
                                aseReport.Eligible = "NO";
                            }


                        }

                    }
                    else
                    {
                        aseReport.Eligible = "NO";
                    }

                    if (item.Sale_Type == "SERVICE")
                    {
                        var matRixDataService = db.ConditionMatrix.Where(x => x.ConditionType == EConditionType.Service && x.RangeStart <= item.TotalCollection && x.RangeEnd >= item.TotalCollection).FirstOrDefault();

                        if (matRixDataService != null)
                        {

                            aseReport.serviceBonus = matRixDataService.FixedAmount;
                        }
                    }


                    aseReports.Add(aseReport);



                }

                foreach (var item in listService)
                {

                    try
                    {
                        //DateTime CollectionDate = item.Invoice_Date;
                        //foreach (var itemDetail in item.Collections)
                        //{
                        //    CollectionDate = itemDetail.PAYMENT_DATE;
                        //}

                        var aseReport = new SC_ASEReport();

                        // aseReport.InvoiceDate = item.Invoice_Date;
                        aseReport.PAYMENT_DOC = item.PAYMENT_DOC;
                        aseReport.Invoice_Doc = item.INVOICE_DOC;
                        aseReport.CollectionDate = item.PAYMENT_DATE;
                        aseReport.Customer_Name = item.CUSTOMER_NAME;
                        aseReport.ASEDisplayName = "";

                        aseReport.Collections = item.AMOUNT;
                        aseReport.Duration = 0;

                        aseReport.SaleType = item.SALE_TYPE;
                        aseReport.lubeQty = 0;
                        aseReport.Eligible = "YES";
                        //if (aseReport.Duration <= 37)
                        //{
                        //    aseReport.Eligible = "YES";
                        //}
                        //else
                        //{
                        //    aseReport.Eligible = "NO";
                        //}

                        if (item.SALE_TYPE == "SERVICE")
                        {
                            var matRixDataService = db.ConditionMatrix.Where(x => x.ConditionType == EConditionType.Service && x.RangeStart <= item.AMOUNT && x.RangeEnd >= item.AMOUNT).FirstOrDefault();

                            if (matRixDataService != null)
                            {

                                aseReport.serviceBonus = matRixDataService.FixedAmount;
                            }
                        }

                        if (item.SALE_TYPE == "TDS_VDS")
                        {
                            var matRixDataTDS_VDS = db.ConditionMatrix.Where(x => x.ConditionType == EConditionType.TDS_VDS && x.RangeStart <= item.AMOUNT && x.RangeEnd >= item.AMOUNT).FirstOrDefault();

                            if (matRixDataTDS_VDS != null)
                            {
                                aseReport.serviceBonus = matRixDataTDS_VDS.FixedAmount;
                            }

                        }



                        aseReports.Add(aseReport);
                    }
                    catch (Exception ex)
                    {

                    }


                }

                if (listTDS != null)
                {
                    foreach (var item in listTDS)
                    {
                        var aseReport = new SC_ASEReport();

                        aseReport.Customer_Name = item.customerName;
                        aseReport.Collections = item.amount;
                        aseReport.SaleType = "TDS";
                        aseReport.Eligible = "YES";

                        var matRixDataTDS = db.ConditionMatrix.Where(x => x.ConditionType == EConditionType.TDS_VDS && x.RangeStart <= item.amount && x.RangeEnd >= item.amount).FirstOrDefault();

                        if (matRixDataTDS != null)
                        {
                            aseReport.serviceBonus = matRixDataTDS.FixedAmount;
                        }

                        aseReports.Add(aseReport);

                    }



                }


                if (listVDS != null)
                {
                    foreach (var item in listVDS)
                    {
                        var aseReport = new SC_ASEReport();

                        aseReport.Customer_Name = item.customerName;
                        aseReport.Collections = item.amount;
                        aseReport.SaleType = "VDS";
                        aseReport.Eligible = "YES";

                        var matRixDataVDS = db.ConditionMatrix.Where(x => x.ConditionType == EConditionType.TDS_VDS && x.RangeStart <= item.amount && x.RangeEnd >= item.amount).FirstOrDefault();

                        if (matRixDataVDS != null)
                        {
                            aseReport.serviceBonus = matRixDataVDS.FixedAmount;
                        }

                        aseReports.Add(aseReport);

                    }



                }









            }

            return aseReports;

        }

        public SC_ASEApprovedData GetASECalculation(IEnumerable<SC_ASEReport> aseReportData, DateTime Start, DateTime End, int ASEID)
        {

            SC_ASEApprovedData calculator = new SC_ASEApprovedData();

            var saleType = aseReportData.Select(x => x.SaleType).Distinct().ToList();

            if (saleType.Contains("SPARE"))
            {
                calculator.spareCollection = aseReportData.Where(x => x.SaleType == "SPARE" && x.Eligible == "YES").Sum(x => x.Claimable_ASE_Collection);

                calculator.spareSalesDuringPeriod = aseReportData.Where(x => x.SaleType == "SPARE").Select(x => x.SpareSalesAmt).FirstOrDefault();

                var ASE_Portion = aseReportData.Where(x => x.SaleType == "SPARE" && x.Eligible == "YES").Sum(x => x.Claimable_ASE_Collection);

                var matRixDataSpare = db.ConditionMatrix.Where(x => x.ConditionType == EConditionType.Spare && x.RangeStart <= ASE_Portion && x.RangeEnd >= ASE_Portion).FirstOrDefault();

                if (matRixDataSpare != null)
                {

                    calculator.spareCommision = Math.Round((ASE_Portion * matRixDataSpare.Perc) / 100, 2);
                    calculator.spareBonus = matRixDataSpare.Bonus;

                }
                else
                {

                    calculator.serviceBonus = 0;
                }

                var totalLubeQty = aseReportData.Where(x => x.SaleType == "SPARE" && x.Eligible == "YES").Sum(x => x.lubeQty);

                var matRixLubeOil = db.ConditionMatrix.Where(x => x.ConditionType == EConditionType.Lube).FirstOrDefault();

                if (matRixLubeOil != null)
                {

                    calculator.lubeOilCollection = totalLubeQty;
                    calculator.lubeOilCommision = (totalLubeQty * matRixLubeOil.FixedAmount);

                }
                else
                {
                    calculator.lubeOilCollection = 0;
                    calculator.lubeOilCommision = 0;

                }






            }


            var month = End.Month;
            var Year = End.Year;


            dynamic aseTargetMonthly = null;

            switch (month)
            {
                case 1:
                    aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.Year == Year && x.MonthYearName == EMonth.January).FirstOrDefault();
                    break;
                case 2:
                    aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.Year == Year && x.MonthYearName == EMonth.February).FirstOrDefault();
                    break;
                case 3:
                    aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.Year == Year && x.MonthYearName == EMonth.March).FirstOrDefault();
                    break;
                case 4:
                    aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.Year == Year && x.MonthYearName == EMonth.April).FirstOrDefault();
                    break;
                case 5:
                    aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.Year == Year && x.MonthYearName == EMonth.May).FirstOrDefault();
                    break;
                case 6:
                    aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.Year == Year && x.MonthYearName == EMonth.June).FirstOrDefault();
                    break;
                case 7:
                    aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.Year == Year && x.MonthYearName == EMonth.July).FirstOrDefault();
                    break;
                case 8:
                    aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.Year == Year && x.MonthYearName == EMonth.Auguest).FirstOrDefault();
                    break;
                case 9:
                    aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.Year == Year && x.MonthYearName == EMonth.September).FirstOrDefault();
                    break;
                case 10:
                    aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.Year == Year && x.MonthYearName == EMonth.October).FirstOrDefault();
                    break;
                case 11:
                    aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.Year == Year && x.MonthYearName == EMonth.November).FirstOrDefault();
                    break;
                case 12:
                    aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.Year == Year && x.MonthYearName == EMonth.December).FirstOrDefault();

                    break;
            }







            //var aseTargetMonthly = db.ASETarget.Where(x => x.TargetType == ETargetType.Monthly && x.ASEUID == ASEID && x.StartDate >= Start && x.EndDate <= End).FirstOrDefault();
            if (aseTargetMonthly != null)
            {
                calculator.spareSalesTarget = aseTargetMonthly.TargetAmount;
                calculator.spareTarget = aseTargetMonthly.AchievedAmount;

                if (calculator.spareCollection > 0 && calculator.spareTarget > 0)
                {
                    calculator.spareCollectionPerCent = Math.Round((calculator.spareCollection / calculator.spareTarget) * 100, 2);
                }

                calculator.spareAdditionalperCent = aseTargetMonthly.AchievedPerc;

                calculator.csaNew = aseTargetMonthly.NewContract;
                calculator.csaExisting = aseTargetMonthly.RenewContract;


                if (calculator.spareCollection >= calculator.spareTarget)
                {
                    calculator.spareAdditionalAmount = Math.Round(((calculator.spareCollection - calculator.spareTarget) / 100) * aseTargetMonthly.AchievedPerc, 2);
                }
                else
                {
                    //calculator.spareTarget = 0;
                    //calculator.spareCollectionPerCent = 0;
                    calculator.spareAdditionalAmount = 0;

                }

                var matRixContractNew = db.ConditionMatrix.Where(x => x.ConditionType == EConditionType.Contract && x.ContractType == EContractType.New).FirstOrDefault();
                var matRixContractExist = db.ConditionMatrix.Where(x => x.ConditionType == EConditionType.Contract && x.ContractType == EContractType.Renew).FirstOrDefault();


                calculator.csaCommision = (calculator.csaNew * matRixContractNew.FixedAmount) + (calculator.csaExisting * matRixContractExist.FixedAmount);


            }





            calculator.serviceBonus = aseReportData.Where(x => x.SaleType == "SERVICE" && x.Eligible == "YES").Sum(x => x.serviceBonus);
            calculator.serviceCollection = aseReportData.Where(x => x.SaleType == "SERVICE").Sum(x => x.Collections);


            calculator.serviceTDSVDSCollection = aseReportData.Where(x => x.SaleType == "TDS").Sum(x => x.Collections) + aseReportData.Where(x => x.SaleType == "VDS").Sum(x => x.Collections);
            calculator.serviceTDSVDSBonus = aseReportData.Where(x => x.SaleType == "TDS" && x.Eligible == "YES").Sum(x => x.serviceBonus) + aseReportData.Where(x => x.SaleType == "VDS" && x.Eligible == "YES").Sum(x => x.serviceBonus);


            //if (saleType.Contains("SERVICE"))
            //{
            //    calculator.serviceCollection = aseReportData.Where(x => x.SaleType == "SERVICE").Sum(x => x.Collections);
            //    var ASE_Portion = aseReportData.Where(x => x.SaleType == "SERVICE").Sum(x => x.ASE_Portion);

            //    var matRixDataService = db.ConditionMatrix.Where(x => x.ConditionType == EConditionType.Service && x.RangeStart <= calculator.serviceCollection && x.RangeEnd >= calculator.serviceCollection).FirstOrDefault();
            //    if (matRixDataService != null)
            //    {

            //        calculator.serviceBonus = matRixDataService.FixedAmount;
            //    }
            //    else
            //    {
            //        calculator.serviceBonus = 0;
            //    }
            //}


            return calculator;
        }


        public User GetUserDetails(int ASEID)
        {
            return db.Users.Where(x => x.UserID == ASEID).FirstOrDefault();
        }


        public void ApprovedASEReport(SC_ASEApprovedData calculation)
        {

            var olDdata = db.SC_ASEApprovedData.Where(x => x.ASEID == calculation.ASEID && x.month == calculation.month && x.year == calculation.year).FirstOrDefault();

            if (olDdata != null)
            {
                olDdata.ASEID = calculation.ASEID;
                olDdata.startDate = calculation.startDate;
                olDdata.endDate = calculation.endDate;
                olDdata.duration = calculation.duration;
                olDdata.spareTarget = calculation.spareTarget;
                olDdata.spareCollection = calculation.spareCollection;
                olDdata.spareCollectionPerCent = calculation.spareCollectionPerCent;
                olDdata.spareCommision = calculation.spareCommision;
                olDdata.spareBonus = calculation.spareBonus;
                olDdata.spareAdditionalperCent = calculation.spareAdditionalperCent;
                olDdata.spareAdditionalAmount = calculation.spareAdditionalAmount;
                olDdata.spareTotalCommision = calculation.spareTotalCommision;
                olDdata.lubeOilCollection = calculation.lubeOilCollection;
                olDdata.lubeOilCommision = calculation.lubeOilCommision;
                olDdata.lubeOilBonus = calculation.lubeOilBonus;
                olDdata.serviceCollection = calculation.serviceCollection;
                olDdata.serviceBonus = calculation.serviceBonus;
                olDdata.serviceTDSVDSCollection = calculation.serviceTDSVDSCollection;
                olDdata.serviceTDSVDSBonus = calculation.serviceTDSVDSBonus;
                olDdata.csaNew = olDdata.csaNew;
                olDdata.csaExisting = calculation.csaExisting;
                olDdata.csaCommision = calculation.csaCommision;
                olDdata.totalCommision = calculation.totalCommision;
                olDdata.month = calculation.month;
                olDdata.year = calculation.year;
                olDdata.BasicSalary = calculation.BasicSalary;
                olDdata.ConveyanceAllowance = calculation.ConveyanceAllowance;
                olDdata.FoodAllowance = calculation.FoodAllowance;
                olDdata.MobileAllowance = calculation.MobileAllowance;
                olDdata.AdditionalAllowance = calculation.AdditionalAllowance;
                olDdata.DeductionAllowance = calculation.DeductionAllowance;
                olDdata.spareSalesTarget = calculation.spareSalesTarget;

                if (calculation.status == 2)
                {
                    olDdata.PMAppDate = DateTime.Now;
                    olDdata.PMUserId = calculation.UserID;

                }
                else if (calculation.status == 3)
                {
                    olDdata.HODAppDate = DateTime.Now;
                    olDdata.HODUserId = calculation.UserID;
                }

                olDdata.status = calculation.status;


            }
            else
            {
                db.SC_ASEApprovedData.Add(calculation);
            }


        }

        public void UpdateCollectionStatus(IEnumerable<SC_ASEReport> ASEReport, DateTime startDate, DateTime endDate, int ASEID, int userType)
        {

            var invoiceDoc = ASEReport.Where(x => x.Eligible == "YES").Select(x => x.Invoice_Doc).ToList();

            var dataSpare_Service = db.SC_Collection.Where(x => invoiceDoc.Contains(x.INVOICE_DOC) && x.SALE_TYPE != "TDS" && x.SALE_TYPE != "VDS" && x.PAYMENT_DATE >= startDate && x.PAYMENT_DATE <= endDate).ToList();
            var dataTDS = db.SC_Collection.Where(x => x.SALE_TYPE == "TDS" && x.PAYMENT_DATE >= startDate && x.PAYMENT_DATE <= endDate && x.ASEUID == ASEID).ToList();

            var dataVDS = db.SC_Collection.Where(x => x.SALE_TYPE == "VDS" && x.PAYMENT_DATE >= startDate && x.PAYMENT_DATE <= endDate && x.ASEUID == ASEID).ToList();

            dataSpare_Service.ForEach(x => { x.isApproved = true; x.status = userType; });
            dataTDS.ForEach(x => { x.isApproved = true; x.status = userType; });

            dataVDS.ForEach(x => { x.isApproved = true; x.status = userType; });


            db.SaveChanges();

            //foreach(var item in ASEReport)
            //{
            //    var data=db.SC_Collection.Where(x=>x.INVOICE_DOC==item.Invoice_Doc && x.SALE_TYPE!="TDS" && x.SALE_TYPE!="VDS").fir
            //}
        }

        public IEnumerable<SC_ASEApprovedData> GetASEApprovedData(UserInfo UI, DateTime start, DateTime End, int ASEID)
        {
            dynamic result = null;
            var userids = user.GetAllUser(UI.User.UserID, new List<int>());

            if(ASEID==0)
            {
                result = from row in db.SC_ASEApprovedData
                         where row.startDate >= start && row.endDate <= End
                         && userids.Contains(row.ASEID) 
                         && row.status>=(int)UI.User.userType
                         select row;
            }
            else
            {
                result = from row in db.SC_ASEApprovedData
                         where row.startDate >= start && row.endDate <= End
                         && row.ASEID==ASEID
                         && row.status >= (int)UI.User.userType
                         select row;
            }
           

            return result;
        }


        public int CheckApproved(UserInfo UI, DateTime start, DateTime End, int ASEID)
        {
            var data = db.SC_ASEApprovedData.Where(x => x.ASEID == ASEID && x.month == End.Month && x.year == End.Year && x.status>=(int)UI.User.userType).ToList();

            if (data.Count > 0)
                return 1;
            else
                return 0;
        }

        public int RejectInvoice(UserInfo UI, string paymentDoc, string invoiceDoc, string saleType, string customername, int ASEID,DateTime Start,DateTime End)
        {
            int userType = (int)UI.User.userType;

            if (saleType == "SPARE")
            {
                var SpareData = db.SC_Collection.Where(x => x.INVOICE_DOC == invoiceDoc && x.SALE_TYPE == "SPARE" && x.PAYMENT_DATE>=Start && x.PAYMENT_DATE<=End).ToList();

                SpareData.ForEach(x => { x.isApproved = false; x.status = 9; x.rejectDateTime = DateTime.Now;x.rejectBy=UI.User.UserID; });
                
                
            }
            else if (saleType == "SERVICE")
            {
                var serviceData = db.SC_Collection.Where(x => x.PAYMENT_DOC == paymentDoc && x.INVOICE_DOC == invoiceDoc && x.ASEUID == ASEID && x.SALE_TYPE == "SERVICE").FirstOrDefault();
                serviceData.status = 9;
                serviceData.isApproved = false;
                serviceData.rejectBy=UI.User.UserID;
                serviceData.rejectDateTime=DateTime.Now;
            }
            else if (saleType == "VDS")
            {
                var VDSData = db.SC_Collection.Where(x => x.CUSTOMER_NAME == customername && x.ASEUID == ASEID && x.SALE_TYPE == "VDS" && x.PAYMENT_DATE>=Start && x.PAYMENT_DATE<=End).ToList();

                VDSData.ForEach(x => { x.isApproved = false; x.status = 9;x.rejectDateTime = DateTime.Now;x.rejectBy=UI.User.UserID; });
                
            
            }
            else if(saleType=="TDS")
            {
                var TDSData = db.SC_Collection.Where(x => x.CUSTOMER_NAME == customername && x.ASEUID == ASEID && x.SALE_TYPE == "TDS" && x.PAYMENT_DATE >= Start && x.PAYMENT_DATE <= End).ToList();
                TDSData.ForEach(x => { x.isApproved = false; x.status = 9; x.rejectDateTime = DateTime.Now; x.rejectBy = UI.User.UserID; });
            }


       
           


            return 0;
        }

        public IEnumerable<setupCalender> GetCalenderData(int year,int month)
        {
            var data = db.setupCalender.Where(x => x.year == year && x.month==month).ToList();
            return data;
        }

    }
}