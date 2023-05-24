using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.Interfaces
{
    public interface IConditionMatrixRepository:IDisposable
    {
        #region Condition Matrix
        SC_ConditionMatrix Get(int ID);
        IEnumerable<SC_ConditionMatrix> GetAll(EBusinessUnit BU);
        void Add(SC_ConditionMatrix obj);
        void Update(SC_ConditionMatrix Obj);
        
        #endregion

        #region ASE Target
        SC_ASETarget GetAT(int ID);
        IEnumerable<SC_ASETarget> GetAllAT(EBusinessUnit BU);
        void AddAT(SC_ASETarget obj);

        void AddUpdateAT(SC_ASETarget obj);
        void UpdateAT(SC_ASETarget Obj);

        void CalenderAddUpdate(setupCalender objCalender,UserInfo UI);
        IEnumerable<setupCalender> ListofYear();
        IEnumerable<setupCalender> ListofYears(int year);
        #endregion
        int GetASEID(string userName);
        void Save();
    }
}
