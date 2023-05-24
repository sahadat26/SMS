using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using SMS.Interfaces;

namespace SMS.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/
        private IJobRepository job { get; set; }
        private IProductRepository product { get; set; }
        private ICustomerRepository customer { get; set; }
        private IUserRepository user { get; set; }
        private IContractRepository contract { get; set; }

        public DashboardController(IJobRepository _job, IProductRepository _product,
            ICustomerRepository _customer,IUserRepository _user,
            IContractRepository _contract)
        {
            job = _job;
            customer = _customer;
            user = _user;
            product = _product;
            contract = _contract;
        }
       
        [CustomAuthorize]
        public ActionResult Index(DateTime? Start, DateTime? End, EFilterUser FilterUser = 0)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            
            if (Start == null && End == null)
            {
                Start = DateTime.Today.AddMonths(-3);
                End = DateTime.Today;
            }

            ViewBag.NoQ = job.GetAllQuery(UI).Where(q=>q.QueryDate>=Start&&q.QueryDate<=End).Count();
            ViewBag.NoP = job.GetAllQuery(UI).Where(q=>q.State==EState.Pending).Where(q => q.QueryDate >= Start && q.QueryDate <= End).Count();
            ViewBag.NoJ = job.GetAllJob(UI).Where(j => j.State == EState.Progress).Where(q => q.QueryDate >= Start && q.QueryDate <= End).Count();
            ViewBag.NoCJ = job.GetAllJob(UI).Where(j => j.State == EState.Finished).Where(q => q.QueryDate >= Start && q.QueryDate <= End).Count();
            ViewBag.StartDate = Start.Value.ToString("MM/dd/yyyy");
            ViewBag.EndDate = End.Value.ToString("MM/dd/yyyy");
            ViewBag.FilterUser = FilterUser;
            

            if(FilterUser==EFilterUser.ByAssignedJob&& job.GetAssignedJob(UI.User.UserID).Where(j=>j.State==EState.Finished).Count()>0)
            {

                var userids = user.GetAllUser(UI.User.UserID,new List<int>());
                var jobs = job.GetAssignedJob(UI.User.UserID).Where(j => j.State == EState.Finished).Where(q => q.JobCardCreateDate >= Start 
                    && q.JobCardCreateDate <= End);
                
                return View(jobs);
            }
            else if (FilterUser == EFilterUser.ByLoggedUser && job.GetAllJob(UI).Where(j => j.State == EState.Finished).Count() > 0)
            {

                var userids = user.GetAllUser(UI.User.UserID, new List<int>());
                var jobs = job.GetAllJob(UI).Where(j => j.State == EState.Finished).Where(q => q.JobCardCreateDate >= Start
                    && q.JobCardCreateDate <= End);

                return View(jobs);
            }
            
            return View(new List<Job>());
        }

        [CustomAuthorize]
        public ActionResult IndexCom()
        {
            return View();
        }

	}
}