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

namespace SMS.Repositories
{
    public class ConditionMatrixRepository:IConditionMatrixRepository
    {
        #region Properties
        private SMSContext db { get; set; }
        public ConditionMatrixRepository(SMSContext _db)
        {
            db = _db;
        }
        public ConditionMatrixRepository()
            : this(new SMSContext())
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

        #region Condition Matrix
        public SC_ConditionMatrix Get(int ID)
        {
            return db.ConditionMatrix.Find(ID);
        }

        public IEnumerable<SC_ConditionMatrix> GetAll(EBusinessUnit BU)
        {
            return db.ConditionMatrix.Where(m => m.BU == BU);
        }

        public void Add(SC_ConditionMatrix obj)
        {
            if (GetAll(obj.BU).Any(c => c.ConditionType == obj.ConditionType && c.SL == obj.SL))
            {
                throw new Exception("Ther serial: " + obj.SL + " exist for " + Enum.GetName(typeof(EConditionType), obj.ConditionType));
            }
            db.ConditionMatrix.Add(obj);
        }

        public void Update(SC_ConditionMatrix Obj)
        {
            var oldone = Get(Obj.ID);
            if (oldone != null)
            {
                if (GetAll(Obj.BU).Any(c => c.ID != Obj.ID && c.ConditionType == Obj.ConditionType && c.SL == Obj.SL))
                {
                    throw new Exception("Ther serial: " + Obj.SL + " exist for " + Enum.GetName(typeof(EConditionType), Obj.ConditionType));
                }
                oldone.Bonus = Obj.Bonus;
                oldone.ConditionType = Obj.ConditionType;
                oldone.ContractType = Obj.ContractType;
                oldone.EntryDateTime = Obj.EntryDateTime;
                oldone.FixedAmount = Obj.FixedAmount;
                oldone.HostName = Obj.HostName;
                oldone.LtrQty = Obj.LtrQty;
                oldone.Perc = Obj.Perc;
                oldone.RangeEnd = Obj.RangeEnd;
                oldone.RangeStart = Obj.RangeStart;
                oldone.SL = Obj.SL;
                oldone.UserID = Obj.UserID;
            }
        }
        #endregion

        #region ASE Target
        public SC_ASETarget GetAT(int ID)
        {
            return db.ASETarget.Find(ID);
        }
        public IEnumerable<SC_ASETarget> GetAllAT(EBusinessUnit BU)
        {
            return db.ASETarget.Where(c=>c.BU==BU);
        }
        public void AddAT(SC_ASETarget obj)
        {
            if(GetAllAT(obj.BU).Any(c=>c.TargetType==obj.TargetType&&c.ASEUID==obj.ASEUID&&c.StartDate<=obj.StartDate&&c.EndDate>=obj.StartDate))
            {
                throw new Exception("The record already exist");
            }
            else
            {
                db.ASETarget.Add(obj);
            }
        }

        public void AddUpdateAT(SC_ASETarget obj)
        {
            if (GetAllAT(obj.BU).Any(c => c.TargetType == obj.TargetType && c.ASEUID == obj.ASEUID && c.MonthYearName==obj.MonthYearName))
            {
               // var oldone = GetAT(obj.ID);
                var oldone = db.ASETarget.Where(x => x.TargetType == obj.TargetType && x.ASEUID == obj.ASEUID && x.MonthYearName == obj.MonthYearName).FirstOrDefault();
                oldone.AchievedAmount = obj.AchievedAmount;
                oldone.AchievedPerc = obj.AchievedPerc;
                oldone.TargetAmount = obj.TargetAmount;
                oldone.ASEUID = obj.ASEUID;
                oldone.EndDate = obj.EndDate;
                oldone.EntryDateTime = obj.EntryDateTime;
                oldone.HostName = obj.HostName;
                oldone.MonthYearName = obj.MonthYearName;
                oldone.StartDate = obj.StartDate;
                oldone.TargetType = obj.TargetType;
                oldone.UserID = obj.UserID;
                oldone.NewContract = obj.NewContract;
                oldone.RenewContract = obj.RenewContract;
                oldone.Year = obj.Year;
            }
            else
            {
                db.ASETarget.Add(obj);
            }
        }

        public void CalenderAddUpdate(setupCalender obj,UserInfo UI)
        {
        
            var allDeletedData = from c in db.setupCalender where c.year==obj.year select c;

            db.setupCalender.RemoveRange(allDeletedData);
            db.SaveChanges();
          

            int i = 0;
            if(obj!=null)
            {
                for(i=1;i<=12;i++)
                {
                    int incMonth = i+1;
                    int fixMonth = 1;
                    int incyear = obj.year + 1;
                    setupCalender calender=new setupCalender();
                    calender.duration=obj.duration;
                    calender.year=obj.year;
                    calender.month=i;
                    calender.EntryBy = UI.User.UserID;
                    if(i<=11)
                    {
                        calender.startDate = Convert.ToDateTime(obj.year.ToString() + '-' + i.ToString() + '-' + "20");
                        calender.endDate = Convert.ToDateTime(obj.year.ToString() + '-' + incMonth.ToString() + '-' + "19");

                       
                    }
                    else if (i == 12)
                    {
                        calender.startDate = Convert.ToDateTime(obj.year.ToString() + '-' + i.ToString() + '-' + "20");
                        calender.endDate = Convert.ToDateTime(incyear.ToString() + '-' + fixMonth.ToString() + '-' + "19");

                      
                    }
                  

                    db.setupCalender.Add(calender);
                }
            }

        }

        public IEnumerable<setupCalender> ListofYear()
        {
            return db.setupCalender.GroupBy(p => new { p.year, p.duration }).Select(g => g.FirstOrDefault())
                .ToList();
        }
        public IEnumerable<setupCalender> ListofYears(int year)
        {
            return db.setupCalender.Where(x => x.year == year).ToList();
              
        }


        public void UpdateAT(SC_ASETarget obj)
        {
            var oldone = GetAT(obj.ID);
            if(oldone!=null)
            {
                if (GetAllAT(obj.BU).Any(c => c.TargetType == obj.TargetType && c.ASEUID == obj.ASEUID 
                    && c.StartDate <= obj.StartDate && c.EndDate >= obj.StartDate&&c.ID!=obj.ID))
                {
                    throw new Exception("The record already exist");
                }
                else
                {
                    oldone.AchievedAmount = obj.AchievedAmount;
                    oldone.AchievedPerc = obj.AchievedPerc;
                    oldone.TargetAmount = obj.TargetAmount;
                    oldone.ASEUID = obj.ASEUID;
                    oldone.EndDate = obj.EndDate;
                    oldone.EntryDateTime = obj.EntryDateTime;
                    oldone.HostName = obj.HostName;
                    oldone.MonthYearName = obj.MonthYearName;
                    oldone.StartDate = obj.StartDate;
                    oldone.TargetType = obj.TargetType;
                    oldone.UserID = obj.UserID;
                    oldone.NewContract = obj.NewContract;
                    oldone.RenewContract = obj.RenewContract;
                    oldone.Year = obj.Year;
                }
            }
        }

        #endregion


        public int GetASEID(string userName)
        {

            return db.Users.Where(x => x.UserName == userName).Select(x => x.UserID).FirstOrDefault();
        }



        public void Save()
        {
            db.SaveChanges();
        }
    }
}