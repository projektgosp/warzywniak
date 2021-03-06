﻿using projekt_gosp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace projekt_gosp.Controllers
{
    [Authorize]
    public class UserPanelController : Controller
    {
        private db context = new db();
        //
        // GET: /UserPanel/

        public ActionResult Index()
        {
            var user = (from p in context.Uzytkownicy
                        where p.ID_klienta == WebSecurity.CurrentUserId
                        select p).FirstOrDefault();
            ViewBag.email = user.Email;

            ViewBag.address = user.Adres;
            if (ViewBag.address == null)
            {
                ViewBag.address = new Adres();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changepassword(string currentpw = "", string newpw = "")
        {
            string username = WebSecurity.CurrentUserName;

            if (currentpw == "" || newpw == "")
            {
                return Json(new { success = "0", error = "Wszystkie pola są wymagane!" }, JsonRequestBehavior.AllowGet);
            }

            if (WebSecurity.ChangePassword(username, currentpw, newpw))
            {
                return Json(new { success = "1" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = "0", error = "Podane hasło jest nieprawidłowe!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changeemail(string currentemail = "", string newemail = "")
        {
            if (newemail == "")
            {
                return Json(new { success = "0", error = "Pole e-mail nie może być puste!" }, JsonRequestBehavior.AllowGet);
            }

            var user = (from p in context.Uzytkownicy
                        where p.ID_klienta == WebSecurity.CurrentUserId
                        select p).FirstOrDefault();
            user.Email = newemail;
            context.SaveChanges();

            return Json(new { success = "1", content = newemail }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changeaddress(Adres adres)
        {
            var user = (from p in context.Uzytkownicy
                        where p.ID_klienta == WebSecurity.CurrentUserId
                        select p).FirstOrDefault();
            user.Adres = adres;
            context.SaveChanges();

            return Json(new { success = "1"}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public int GetMyPoints()
        {
            var user = (from p in context.Uzytkownicy
                        where p.ID_klienta == WebSecurity.CurrentUserId
                        select p).FirstOrDefault();

            return user.Punkty;
        }

        [HttpGet]
        public ActionResult OrderHistory()
        {
            List<Zamowienie> orders = (from p in context.Zamowienia
                          where p.ID_klienta == WebSecurity.CurrentUserId && p.czyPotwierdzonePrzezKlienta == true
                          orderby p.ID_zamowienia descending
                          select p).ToList();
            return View(orders);
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
