using projekt_gosp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace projekt_gosp.Controllers
{
    [Authorize(Roles = "shop")]
    public class SellerPanelController : Controller
    {
        private db context = new db();
        //
        // GET: /ShopPanel/

        public ActionResult Index()
        {
            var user = (from p in context.Uzytkownicy
                        where p.ID_klienta == WebSecurity.CurrentUserId
                        select p).FirstOrDefault();

            var shop = (from p in context.Sklepy
                        where p.ownerID == WebSecurity.CurrentUserId
                        select p).FirstOrDefault();

            ViewBag.email = user.Email;

            ViewBag.address = user.Adres;

            ViewBag.privatePhoneNumber = user.Nr_tel;
            ViewBag.shopPhoneNumber = shop.Nr_tel;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changephonenumbers(string privatePhoneNumber = null, string shopPhoneNumber = null) 
        {
            if (privatePhoneNumber != null)
            {
                var user = (from p in context.Uzytkownicy
                            where p.ID_klienta == WebSecurity.CurrentUserId
                            select p).FirstOrDefault();

                user.Nr_tel = privatePhoneNumber;
            }

            if (shopPhoneNumber != null)
            {
                var shop = (from p in context.Sklepy
                            where p.ownerID == WebSecurity.CurrentUserId
                            select p).FirstOrDefault();

                shop.Nr_tel = shopPhoneNumber;
            }

            context.SaveChanges();

            return Json(new { success = "1"}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changeemail(string currentemail = "", string newemail = "")
        {
            if (newemail == "")
            {
                return Json(new { success = "0", error = "Email field can not be empty" }, JsonRequestBehavior.AllowGet);
            }

            var user = (from p in context.Uzytkownicy
                        where p.ID_klienta == WebSecurity.CurrentUserId
                        select p).FirstOrDefault();
            user.Email = newemail;

            var shop = (from p in context.Sklepy
                        where p.ownerID == WebSecurity.CurrentUserId
                        select p).FirstOrDefault();

            shop.Email = newemail;

            context.SaveChanges();

            return Json(new { success = "1", content = newemail }, JsonRequestBehavior.AllowGet);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
