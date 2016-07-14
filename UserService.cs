using EmployeePortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace EmployeePortal.Services
{
    public class UserService : Controller
    {
        //public void GetApplicationUser(out ApplicationUser user, out UserManager<ApplicationUser> uManager)
        //{
        //    ApplicationDbContext db = new ApplicationDbContext();
        //    user = new ApplicationUser();
        //    UserStore<ApplicationUser> uStore = new UserStore<ApplicationUser>(db);
        //    uManager = new UserManager<ApplicationUser>(uStore);
        //}

        public UserManager<ApplicationUser> GetUserManager()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            UserStore<ApplicationUser> uStore = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> uManager = new UserManager<ApplicationUser>(uStore);
            return uManager;
        }
    }
}