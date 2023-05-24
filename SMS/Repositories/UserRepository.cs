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
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.Entity.Validation;

namespace SMS.Repositories
{
    public class UserRepository:IUserRepository
    {
        private SMSContext db { get; set; }
        
        public UserRepository(SMSContext _db)
        {
            db = _db;
        }
        public UserRepository()
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

        public User Get(int ID)
        {
            return db.Users.Find(ID);
        }

        public IEnumerable<User> GetAll(UserInfo UI)
        {
            if(UI.BusinessUnit==EBusinessUnit.ALL)
            
                return db.Users.Where(u=>u.UserName!="admin");
            return db.Users.Where(u => u.UserName != "admin" && u.BusinessUnit == UI.BusinessUnit&&u.Exist==true);
        }
        public List<int> GetAllUser(int UserID,List<int> userids)
        {
            if(!userids.Any(id=>id==UserID))
            {
                userids.Add(UserID);
            }
            var childs= db.Users.Where(u => u.SupervisorID == UserID).Select(u => u.UserID).ToList();
            foreach(var u in childs)
            {
                userids.Add(u);
                userids = GetAllUser(u,userids);
            }
            return userids;
            
        }
        public void Add(User obj)
        {
            DuplicateCheck(obj.UserName, "UserName");
            //if(obj.Email!=null)
            //{
            //    DuplicateCheck((obj.Email??""), "Email");
            //}
            var hash = CommonMethod.GetMD5(obj.Password.Trim());
            obj.ConfirmPassword = hash;
            obj.Password = hash;
            obj.ProfitCenter = OrgAll(obj.BusinessUnit).FirstOrDefault().ProfitCenter;
            db.Users.Add(obj);
        }
        public async Task<IEnumerable<VMEmployee>> SyncEmployee(string PC)
        {
            IList<VMEmployee> emps = new List<VMEmployee>();
            try
            {

                string uri = "http://192.168.0.8:96/api/EmployeeAPI/GetEmployeeMasters";
                HttpClient _client = new HttpClient();
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    emps = JsonConvert.DeserializeObject<List<VMEmployee>>(content);
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
                throw new Exception(errMsg);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message.ToString());
            }
            return emps.Where(m=>m.ProfitCenter==PC);
        }
        private void DuplicateCheck(string id, string field)
        {
            if (field == "UserName")
            {
                if (db.Users.Any(gu => gu.UserName == id))
                {
                    throw new Exception(id + " already exist");
                }
            }
            else if (field == "Email"&&id!="")
            {
                if (db.Users.Any(gu => gu.Email == id))
                {
                    throw new Exception(id + " already exist");
                }
            }
        }

        public void Update(User Obj)
        {
            var oldemp = Get(Obj.UserID);
            if (Obj.UserName != oldemp.UserName)
            {
                DuplicateCheck(Obj.UserName, "UserName");
            }
            //else if (Obj.Email!=null&& Obj.Email != oldemp.Email)
            //{
            //    DuplicateCheck((Obj.Email ?? ""), "Email");
            //}
            oldemp.FullName = Obj.FullName;
            oldemp.Email = Obj.Email;
            oldemp.UserName = Obj.UserName;
            oldemp.Department = Obj.Department;
            oldemp.BusinessUnit = Obj.BusinessUnit;
            oldemp.CreateDate = Obj.CreateDate;
            oldemp.FullName = Obj.FullName;
            oldemp.SupervisorID = Obj.SupervisorID;
            oldemp.ConfirmPassword = oldemp.Password;
            oldemp.DesignationID = Obj.DesignationID;
            oldemp.ProfitCenter = Obj.ProfitCenter;
            oldemp.BasicSalary = Obj.BasicSalary;
            oldemp.ConveyanceAllowance = Obj.ConveyanceAllowance;
            oldemp.FoodAllowance = Obj.FoodAllowance;
            oldemp.MobileAllowance = Obj.MobileAllowance;
            oldemp.AdditionalAllowance = Obj.AdditionalAllowance;
            oldemp.DeductionAllowance = Obj.DeductionAllowance;
            oldemp.Wing = Obj.Wing;
            oldemp.userType = Obj.userType;
        }
        public Role GetRole(int ID)
        {
            return db.Roles.Find(ID);
        }
        public bool CheckRoleExistant(int UserID, String RoleName)
        {
            var u = Get(UserID);
            return u.Roles.Any(r=>r.Name==RoleName);
        }
        public void Disable(int ID)
        {
            var gu = Get(ID);
            if (gu != null)
            {
                gu.Exist = false;
            }
            else
            {
                throw new Exception("No user found");
            }
        }
        public void ActiveDeactive(int UID)
        {
            var gu = Get(UID);
            if (gu != null)
            {
                gu.ConfirmPassword = gu.Password;
                if(gu.Exist==true)
                {
                    gu.Exist = false;
                }
                else
                {
                    gu.Exist = true;
                }
            }
            else
            {
                throw new Exception("No user found");
            }
        }
        public SelectList GetASE(UserInfo UI)
        {
            var aselist = GetAll(UI).Where(c=>c.IsASE==true);
            return new SelectList(aselist.OrderBy(m=>m.UserName),"UserID","DisplayText");
        }
        public void ASEUpdate(int UID)
        {
            var gu = Get(UID);
            if (gu != null)
            {
                gu.ConfirmPassword = gu.Password;
                if (gu.IsASE == true)
                {
                    gu.IsASE = false;
                }
                else
                {
                    gu.IsASE = true;
                }
            }
            else
            {
                throw new Exception("No user found");
            }
        }
        public User GetUserBySAPCode(string SAPCode)
        {
            if(SAPCode=="")
            {
                return new User();
            }
            var UserName = SAPCode.Replace("EP","");
            UserName= UserName.Replace("-","");
            var u = db.Users.Where(s => s.UserName == UserName).FirstOrDefault();
            return u;
        }
        public SelectList GetSelectList(UserInfo UI)
        {
            return new SelectList(GetAll(UI).Where(gu =>gu.Exist == true)
                .OrderBy(gu => gu.FullName), "UserID", "DisplayText");
        }


        public SelectList GetSelectListByUser(int UID)
        {
            var userids = GetAllUser(UID,new List<int>());
            var users = from row in db.Users
                        join prow in userids on
                        row.UserID equals prow
                        select row;
            return new SelectList(users.OrderBy(d=>d.FullName),"UserID","DisplayText");
        }
        public SelectList GetDepartments(UserInfo UI)
        {
            
            var department = (from row in GetAll(UI)
                             select new
                             {
                                 ID = row.Department,
                                 Name = row.Department
                             }).Distinct();
            return new SelectList(department.OrderBy(c=>c.Name),"ID","Name");
        }
        
        public void SetPassword(User user, String p)
        {
            var password = CommonMethod.GetMD5(p.Trim());
            user.Password = password;
            user.ConfirmPassword = password;
        }
        public bool ValidateUser(string UserName,string Password)
        {
            bool flag = false;
            if (UserName != null || Password != null)
            {
                var hash = CommonMethod.GetMD5(Password.Trim());
                flag = db.Users.Any(gu => gu.Exist == true &&
                    gu.UserName == UserName && gu.Password == hash);
            }
            else
            {
                throw new Exception("Invalid request!");
            }
            return flag;
        }
        public bool CheckUser(int LoggedUserID, int ObjectUserID)
        {
            if (LoggedUserID == ObjectUserID)
                return true;
            var userids = GetAllUser(LoggedUserID, new List<int>());
            return userids.Any(u => u == ObjectUserID);
        }
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword) || string.IsNullOrEmpty(newPassword.Trim()))
                return false;

            User user = db.Users.Where(c=>c.UserName==username).FirstOrDefault();
            string hash = CommonMethod.GetMD5(newPassword.Trim());

            user.Password = hash;
            user.ConfirmPassword = hash;
            Save();

            return true;
        }
        #region Designation
        public Designation GetD(int DID)
        {
            return db.Designations.Find(DID);
        }
        public Designation GetDByGrade(string JobGrade)
        {
            return db.Designations.Where(m => m.JobGrade == JobGrade).FirstOrDefault();
        }
        public IEnumerable<Designation> GetAllD(EBusinessUnit BU)
        {
            if (BU == EBusinessUnit.ALL)
                return db.Designations;
            return db.Designations.Where(d=>d.Name!="Administrator"&&d.BU==BU);
        }
        public void AddD(Designation obj)
        {
            if (db.Designations.Any(d => d.Name == obj.Name&&d.BU==obj.BU))
                throw new Exception("The designation exist");
            obj.JobGrade = obj.JobGrade.ToUpper();
            db.Designations.Add(obj);
        }
        public void UpdateD(Designation obj)
        {
            var oldone = GetD(obj.DesignationID);
            if (oldone.Name!=obj.Name&& db.Designations.Any(d => d.Name == obj.Name&&d.BU==obj.BU))
                throw new Exception("The designation exist");
            oldone.EntryDateTime = obj.EntryDateTime;
            oldone.FoodAllowance = obj.FoodAllowance;
            oldone.HolidayHourRate = obj.HolidayHourRate;
            oldone.HostName = obj.HostName;
            oldone.Name = obj.Name;
            oldone.OTHourRate = obj.OTHourRate;
            oldone.DailyAllowance = obj.DailyAllowance;
            oldone.DailyAllowanceCV = obj.DailyAllowanceCV;
            oldone.MCAccomodation = obj.MCAccomodation;
            oldone.OCAccomodation = obj.OCAccomodation;
            oldone.UserID = obj.UserID;
            oldone.WorkingHourRate = obj.WorkingHourRate;
            oldone.JobGrade = obj.JobGrade.ToUpper();
        }
        public SelectList GetDesignations(EBusinessUnit BU)
        {
            var list = GetAllD(BU).OrderBy(d => d.Name);
            return new SelectList(list,"DesignationID","Name");
        }
        public void AdjustRate(EBusinessUnit BU, DateTime Start, DateTime End)
        {
            var mhcost = db.ManpowerCosts.Where(mh => mh.Job.BusinessUnit == BU &&
                mh.Job.JobCardCreateDate >= Start && mh.Job.JobCardCreateDate <= End);
            foreach(var mh in mhcost)
            {
                mh.FoodAllowance = mh.Employee.Designation.FoodAllowance;
                mh.HolidayHourRate = mh.Employee.Designation.HolidayHourRate;
                mh.OTHourRate = mh.Employee.Designation.OTHourRate;
                mh.WorkingHourRate = mh.Employee.Designation.WorkingHourRate;
            }
        }
        #endregion

        #region Revenue Wing
        public RevenueWing GetWing(int ID)
        {
            return db.RevenueWings.Find(ID);
        }
        public IEnumerable<RevenueWing> GetWingAll(EBusinessUnit BU)
        {
            return db.RevenueWings.Where(rw=>rw.BU==BU);
        }
        public void AddWing(RevenueWing obj)
        {
            if (db.RevenueWings.Any(p => p.Name == obj.Name))
                throw new Exception(obj.Name + " already exist");
            db.RevenueWings.Add(obj);
        }
        public void UpdateWing(RevenueWing Obj)
        {
            var oldone = GetWing(Obj.RevenueWingID);
            if (oldone.Name != Obj.Name && db.RevenueWings.Any(p => p.Name == Obj.Name))
                throw new Exception(Obj.Name + " already exist");
            oldone.EntryDateTime = Obj.EntryDateTime;
            oldone.HostName = Obj.HostName;
            oldone.Name = Obj.Name;
            oldone.UserID = Obj.UserID;
        }
        public SelectList GetWingSelectlist(EBusinessUnit BU)
        {
            return new SelectList(GetWingAll(BU).OrderBy(a => a.Name), "RevenueWingID", "Name");
        }
        #endregion
        #region Org Setting
        public IEnumerable<OrgSetting> OrgAll(EBusinessUnit BU)
        {
            return db.OrgSetting;
        }
        public OrgSetting OrgSingle(string PC)
        {
            return db.OrgSetting.Find(PC);
        }
        #endregion
        public void Save()
        {
            db.SaveChanges();
        }
    }
}