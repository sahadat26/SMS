using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.Interfaces
{
    public interface IContractRepository:IDisposable
    {
        #region Service Contract
        ServiceContract GetSC(int ID);
        IEnumerable<ServiceContract> GetSCAll(UserInfo UI);
        void AddSC(ServiceContract obj);
        void UpdateSC(ServiceContract obj);
        SelectList SelectListSC(UserInfo UI);
        int ServiceRandFileName(int max, string ext);
        
        void SCAutoClose();
        void RenewContract(int SCID,DateTime Start,DateTime End);
        #endregion
        #region Service Product
        ServiceProduct GetSP(int ID);
        IEnumerable<ServiceProduct> GetSPAll(int SCID);
        void AddSP(ServiceProduct obj);
        ServiceProductDetail GetSPD(int ID);
        IEnumerable<ServiceProductDetail> GetSPDAll();
        List<string> GetSPPropertyValue(int ProductPropertyID);
        void AddSPD(ServiceProductDetail property);
        void ModifySPD(ServiceProductDetail property);
        void UpdateSP(ServiceProduct obj);
        
        #endregion
        #region Service Contract Collection
        ServiceContractCollection GetCol(int ID);
        IEnumerable<ServiceContractCollection> GetColAll(int SCID);
        void AddCol(ServiceContractCollection obj);
        void UpdateCol(ServiceContractCollection obj);

        #endregion
        #region Warranty Contract
        WarrantyContract GetWC(int ID);
        IEnumerable<WarrantyContract> GetWCAll(UserInfo UI);
        void AddWC(WarrantyContract obj);
        void UpdateWC(WarrantyContract obj);
        SelectList SelectListWC(UserInfo UI);
        int WarrantyRandFileName(int max, string ext);
        void WCAutoClose();
        #endregion
        #region Warranty Product
        WarrantyProduct GetWP(int ID);
        IEnumerable<WarrantyProduct> GetWPAll(int WPID);
        void AddWP(WarrantyProduct obj);
        WarrantyProductDetail GetWPD(int ID);
        IEnumerable<WarrantyProductDetail> GetWPDAll();
        void AddWPD(WarrantyProductDetail property);
        void ModifyWPD(WarrantyProductDetail property);
        List<string> GetWPPropertyValue(int ProductPropertyID);
        void UpdateWP(WarrantyProduct obj);
        
        
        #endregion
        void Save();
    }
}
