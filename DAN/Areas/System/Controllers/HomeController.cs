using DAN.Areas.System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DAN.Areas.System.Controllers
{

    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        DAN.Models.DANEntities db = new DAN.Models.DANEntities();
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pass = MyHelpers.Md5(model.Upass);
                var admin = db.Users.SingleOrDefault(e => e.Uname == model.Uname && e.Upass == pass);
                if(admin != null)
                {
                    try
                    {
                        FormsAuthentication.SetAuthCookie(model.Uname, false);
                        var authTicket = new FormsAuthenticationTicket(1, admin.Uname, DateTime.Now, DateTime.Now.AddDays(1), false, admin.Roles);
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        HttpContext.Response.Cookies.Add(authCookie);
                        return RedirectToAction("Index", "Manage");
                    }
                    catch(Exception e)
                    {
                        ModelState.AddModelError("", e.Message);
                        return View(model);
                    }
                }
                ModelState.AddModelError("", "Đăng nhập thất bại!");
            }
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}