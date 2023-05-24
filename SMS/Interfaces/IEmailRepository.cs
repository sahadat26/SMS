using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.Interfaces
{
    public interface IEmailRepository:IDisposable
    {
        EmailText Get(EActions ID);
        IEnumerable<EmailText> GetAll();
        void Add(EmailText obj);
        void Update(EmailText Obj);
        void Delete(EActions ID);
        EmailSetting GetSingleEmailSetting(string name="EPGL");
        IEnumerable<EmailSetting> GetAllSetting();
        void UpdateEmailSetting(EmailSetting Obj);
        bool SendEmail(EmailSetting Obj,string To,string Subject,string Body);
        string SetBody(string body);
        void Save();
    }
}
