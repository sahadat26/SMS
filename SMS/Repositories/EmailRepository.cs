using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMS.Interfaces;
using SMS.Models;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Helpers;

namespace SMS.Repositories
{
    public class EmailRepository:IEmailRepository
    {
        private SMSContext db { get; set; }
        
        public EmailRepository(SMSContext _db)
        {
            db = _db;
        }
        public EmailRepository()
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


        public EmailText Get(EActions ID)
        {
            return db.EmailTexts.Find(ID);
        }

        public IEnumerable<EmailText> GetAll()
        {
            return db.EmailTexts;
        }

        public void Add(EmailText obj)
        {
            db.EmailTexts.Add(obj);
        }

        public void Update(EmailText Obj)
        {
            db.Entry(Obj).State = EntityState.Modified;
        }

        public void Delete(EActions ID)
        {
            var ObjToDelete = Get(ID);
            db.EmailTexts.Remove(ObjToDelete);
        }
        #region Email Setting
        public EmailSetting GetSingleEmailSetting(string name="EPGL")
        {
            return db.EmailSettings.Find(name);
        }
        public IEnumerable<EmailSetting> GetAllSetting()
        {
            return db.EmailSettings;
        }
        public void UpdateEmailSetting(EmailSetting Obj)
        {
            db.Entry(Obj).State = EntityState.Modified;
        }
        public bool SendEmail(EmailSetting Obj,string To, string Subject, string Body)
        {
            WebMail.EnableSsl = false;
            WebMail.SmtpServer = Obj.SMTPServer;
            WebMail.SmtpPort = Obj.Port;
            WebMail.From = Obj.EmailAddress;
            WebMail.UserName = Obj.UserName;
            WebMail.Password = Obj.Password;

            try
            {
                Body = Body + Obj.Signature;
                WebMail.Send(To,Subject,Body);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string SetBody(string body)
        {
            
            return body;
        }
        #endregion
        public void Save()
        {
            db.SaveChanges();
        }
    }
}