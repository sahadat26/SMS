using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.Interfaces
{
    public interface IUserRepository:IDisposable
    {
        User Get(int ID);
        IEnumerable<User> GetAll(UserInfo UI);
        List<int> GetAllUser(int UserID,List<int> userids);
        void Add(User obj);
        void Update(User Obj);
        SelectList GetSelectList(UserInfo UI);
        SelectList GetSelectListByUser(int UID);
        SelectList GetDepartments(UserInfo UI);
        SelectList GetASE(UserInfo UI);
        void SetPassword(User emp, String p);
        bool ChangePassword(string username, string oldPassword, string newPassword);
        bool ValidateUser(string UserID, string Password);
        bool CheckUser(int LoggedUserID, int ObjectUserID);
        bool CheckRoleExistant(int UserID,String RoleName);
        void ActiveDeactive(int UID);
        void ASEUpdate(int UID);
        Role GetRole(int ID);
        Task<IEnumerable<VMEmployee>> SyncEmployee(string PC);
        User GetUserBySAPCode(string SAPCode);
        #region Designation
        Designation GetD(int DID);
        IEnumerable<Designation> GetAllD(EBusinessUnit BU);
        void AddD(Designation obj);
        void UpdateD(Designation obj);
        SelectList GetDesignations(EBusinessUnit BU);
        void AdjustRate(EBusinessUnit BU, DateTime Start, DateTime End);
        Designation GetDByGrade(string JobGrade);
        
        #endregion

        #region Revenue Wing
        RevenueWing GetWing(int ID);
        IEnumerable<RevenueWing> GetWingAll(EBusinessUnit BU);
        void AddWing(RevenueWing obj);
        void UpdateWing(RevenueWing Obj);
        SelectList GetWingSelectlist(EBusinessUnit BU);
        #endregion

        #region Org Setting
        IEnumerable<OrgSetting> OrgAll(EBusinessUnit BU);
        OrgSetting OrgSingle(string PC);
        #endregion
        void Save();
    }
}
