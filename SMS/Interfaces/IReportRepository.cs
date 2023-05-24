using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.Interfaces
{
    public interface IReportRepository:IDisposable
    {
        #region Service Management
        PerformanceModel GetIndividualPerformance(DateTime Start,DateTime End,int UID);
        IEnumerable<PerformanceModel> GetTeamPerformance(DateTime Start, DateTime End, int UID);
        IEnumerable<BarChartModel> GetRevenueExpenseByProduct(DateTime Start, DateTime End, int UID);
        IEnumerable<BarChartModel> GetRevenueExpenseByWing(DateTime Start, DateTime End, int UID);
        IEnumerable<BarChartModel> GetTopContractByProfit(DateTime Start, DateTime End, int UID);
        IEnumerable<BarChartModel> GetTopContractByLoss(DateTime Start, DateTime End, int UID);
        IEnumerable<BarChartModel> GetMostComplainedWarrantyProduct(DateTime Start, DateTime End, int UID);
        IEnumerable<BarChartModel> SPCloseWithDays(UserInfo UI);
        IEnumerable<BarChartModel> WPExpireWithDays(UserInfo UI);
        IEnumerable<ManpowerCost> GetJobHistory(DateTime Start, DateTime End, int UID);
        IEnumerable<ManpowerCost> GetJobHistoryInDateRange(DateTime Start, DateTime End);
        IEnumerable<BarChartModel> GetJobByProductProperty(DateTime Start, DateTime End, int UID,int PPID);
        IEnumerable<Job> GetJobHistoryByCustomer(DateTime Start, DateTime End, int CID);

        IEnumerable<SC_ASEApprovedData> GetASEApprovedData(UserInfo UI,DateTime start, DateTime End, int ASEID);
        #endregion

        #region ASE Commission
        //IEnumerable<SC_SpareCom> SC_GetSpareSaleCollection(int ASEUID, DateTime Start, DateTime End, int days);
        #endregion
        void Save();
    }
}
