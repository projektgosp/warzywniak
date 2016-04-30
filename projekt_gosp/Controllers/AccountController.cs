using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using projekt_gosp.Models;

namespace shop_online.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private db context = new db();

        [HttpGet]
        public ActionResult login(string ReturnUrl = "")
        {
             
            if (WebSecurity.IsAuthenticated == false)
            {
                ViewBag.ReturnUrl = ReturnUrl;
                return View();
            }
            return RedirectToAction("Index", "Shop");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult login(additionalModels.loginModel user, string ReturnUrl = "")
        {
            if (WebSecurity.IsAuthenticated == false)
            {
                if (ModelState.IsValid)
                {

                    if (WebSecurity.Login(user.username, user.password))
                    {
                        if (ReturnUrl != null)
                        {
                            return Redirect("~/" + ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Shop");
                        }
                    }
                    else
                    {
                        ViewBag.error = "username or/and password is incorrect";
                    }

                    ViewBag.ReturnUrl = ReturnUrl;
                    ViewBag.username = user.username;

                    return View();
                }
                return View();
            }
            return RedirectToAction("Index", "Shop");
        }

        public ActionResult register()
        {
            if (WebSecurity.IsAuthenticated == false)
            {
                return View();
            }
            return RedirectToAction("Index", "Shop");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult register(additionalModels.registerModel user)
        {
            if (WebSecurity.IsAuthenticated == false)
            {
                if (ModelState.IsValid)
                {
                    if (IsMailInUse(user.email))
                    {
                        ViewBag.error = "Podany adres email jest już zajęty";
                        return View(user);
                    }

                    WebSecurity.CreateUserAndAccount(user.username, user.password, new
                    {
                        Email = user.email,
                    }, false);

                    Roles.AddUserToRole(user.username, "customer");

                    ViewBag.error = "Twoje konto zostało pomyślnie utworzone";
                }
                return View();
            }
            return RedirectToAction("Index", "Shop");
        }

        public ActionResult registerShop()
        {
            if (WebSecurity.IsAuthenticated)
            {
                return View();
            }

            return RedirectToAction("Index", "Shop");
        }

        //[Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult registerShop(additionalModels.registerShopModel shop)
        {
            if (WebSecurity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    if (IsMailInUse(shop.email))
                    {
                        ViewBag.error = "Podany adres email jest już zajęty";
                        return View(shop);
                    }
                    try
                    {
                        WebSecurity.CreateUserAndAccount(shop.username, shop.password, new
                        {
                            Email = shop.email,
                            Imie = shop.name,
                            Nazwisko = shop.surname,
                            Nr_tel = shop.privatePhoneNumber
                        }, false);

                        int userID = WebSecurity.GetUserId(shop.username);

                        Roles.AddUserToRole(shop.username, "shop");

                        Adres address = new Adres
                        {
                            Miasto = shop.city,
                            Kod_pocztowy = shop.postalCode,
                            Ulica = shop.street,
                            Nr_budynku = shop.streetNumber,
                            Nr_lokalu = shop.flatNumber
                        };

                        context.Adresy.Add(address);

                        Sklep newShop = new Sklep
                        {
                            Email = shop.email,
                            Nr_tel = shop.shopPhoneNumber,
                            ownerID = userID,
                            Adres = address
                        };

                        context.Sklepy.Add(newShop);

                        context.SaveChanges();

                        Uzytkownik createdUser = (from p in context.Uzytkownicy
                                                  where p.ID_klienta == userID
                                                  select p).FirstOrDefault();

                        createdUser.Adres = address;

                        createdUser.selectedShopId = newShop.ID_sklepu;
 
                        context.SaveChanges();

                        ViewBag.error = "Konto sklepu zostało pomyślnie utworzone";
                    }
                    catch(Exception ex)
                    {

                    }

                }
                return View();
            }
            return RedirectToAction("Index", "Shop");
        }


        private bool IsMailInUse(string email)
        {
            var user = (from p in context.Uzytkownicy
                        where p.Email == email
                        select p).FirstOrDefault();

            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [Authorize]
        public ActionResult logout()
        {
            WebSecurity.Logout();
            return RedirectToAction("Index", "Shop");
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