using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.Interfaces
{
    public interface ISalesCommissionRepository:IDisposable
    {
        #region Sales
        SC_Sales GetSale(string  Invoice_Doc);
        IEnumerable<SC_Sales> GetAllSales(UserInfo UI);
        IEnumerable<SC_Sales> GetAllSales();
        void AddSale(SC_Sales obj,UserInfo UI);
        Task<IEnumerable<SC_Sales>> SyncSales(string SalesOrg, DateTime fromdt, DateTime todt, string Div, string flag="sale");
        bool IsUserPermitted(string Invoice,UserInfo UI);
        void ASEPortionUpdate(VMASEPart obj);

        IEnumerable<SC_ASEReport> GetASEData(UserInfo UI, DateTime Start, DateTime End, int ASEID, string duration);
        SC_ASEApprovedData GetASECalculation(IEnumerable<SC_ASEReport> aseReportData, DateTime Start, DateTime End, int ASEID);
        User GetUserDetails(int ASEID);
        void ApprovedASEReport(SC_ASEApprovedData calculation);
        IEnumerable<setupCalender> GetCalenderData(int year,int month);
        #endregion
        #region Collection
        SC_Collection GetCollection(int ID);
        IEnumerable<SC_Collection> GetAllCollection(UserInfo UI);
        IEnumerable<SC_Collection> GetAllCollection();
        void AddCollection(SC_Collection obj, UserInfo UI);
        void SetLastPayment(int ID);
        void SetColASE(VMColASE obj,UserInfo UI);
        Task<IEnumerable<SC_Collection>> SyncCollection(string SalesOrg, DateTime fromdt, DateTime todt, string Div, string flag = "payment");
        #endregion
        #region ASE Contribution
        SC_ASEContribution GetAC(int ID);
        IEnumerable<SC_ASEContribution> GetACBySales(string Invoice_Doc);
        IEnumerable<SC_ASEContribution> GetAllAC(UserInfo UI);
        IEnumerable<SC_ASEContribution> GetAllAC();
        void AddAC(IList<SC_ASEContribution>  obj,UserInfo UI);
        void UpdateAC(IList<SC_ASEContribution>  obj,UserInfo UI);
        void UpdateCollectionStatus(IEnumerable<SC_ASEReport> ASEReport, DateTime startDate, DateTime endDate, int ASEID, int userType);
        IEnumerable<SC_ASEApprovedData> GetASEApprovedData(UserInfo UI, DateTime start, DateTime End, int ASEID);

        int CheckApproved(UserInfo UI, DateTime start, DateTime End, int ASEID);
        int RejectInvoice(UserInfo UI, string paymentDoc, string invoiceDoc, string saleType, string customername, int ASEID,DateTime Start,DateTime End);
        #endregion

        void Save();
    }
}
