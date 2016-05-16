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
using projekt_gosp.Helpers;

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
                            return RedirectToAction("Index", "shop");
                        }
                    }
                    else
                    {
                        if (WebSecurity.UserExists(user.username))
                        {
                            if (!WebSecurity.IsConfirmed(user.username))
                            {
                                ViewBag.error = "check your email to confirm your account";
                            }
                            else
                            {
                                ViewBag.error = "username or/and password is incorrect";
                            }
                        }
                        else
                        {
                            ViewBag.error = "username or/and password is incorrect";
                        }
                    }

                    ViewBag.ReturnUrl = ReturnUrl;
                    ViewBag.username = user.username;

                    return View();
                }
                return View();
            }
            return RedirectToAction("Index", "shop");
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

                    try
                    {
                        string confirmToken = WebSecurity.CreateUserAndAccount(user.username, user.password, new
                        {
                            Email = user.email,
                        }, true);

                        string callbackUrl = CreateConfirmUrl(user.username, confirmToken);

                        var subject = "Welcome to grocery shop";
                        var body = String.Format("Hello {0}, please confirm your registration by clicking the following link: <a href=\"{1}\">Confirm</a>", user.username, callbackUrl);

                        GlobalMethods.SendMailThread(user.email, subject, body);

                        ViewBag.error = "Thank you for registering. A validation e-mail has been sent to your e-mail address : " + user.email +
                            " Please check your email and use the enclosed link to finish registration.";
                        ViewBag.username = "";
                        ViewBag.email = "";

                        Roles.AddUserToRole(user.username, "customer");
                        return View();
                    }
                    catch (System.Web.Security.MembershipCreateUserException e)
                    {
                        ViewBag.error = e.Message;
                        return View();
                    }
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

        [HttpGet]
        public ActionResult confirm(string username = "", string token = "")
        {
            if (WebSecurity.IsAuthenticated == false)
            {
                if (username == "" || token == "")
                {
                    return View();
                }

                if (WebSecurity.IsConfirmed(username))
                {
                    var loginUrl = Url.Action(
                        "login", "Account");
                    ViewBag.msg = string.Format("Your account has been already activated!</br> Click here to login into your account : <a href='{0}'>Login</a>", loginUrl);
                }
                else
                {
                    if (WebSecurity.ConfirmAccount(username, token))
                    {
                        var loginUrl = Url.Action(
                            "login", "Account");
                        ViewBag.msg = string.Format("Your account has been activated!</br> Click here to login into your account : <a href='{0}'>Login</a>", loginUrl);
                    }
                    else
                    {
                        ViewBag.msg = "There was problem while confirming your account.";
                    }
                }
                return View();
            }
            return RedirectToAction("Index", "shop");
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

        public ActionResult Rules() { return View(); }

        [Authorize]
        public ActionResult logout()
        {
            WebSecurity.Logout();
            return RedirectToAction("Index", "Shop");
        }

        private string CreateConfirmUrl(string username, string token)
        {
            var callbackUrl = Url.Action(
               "confirm", "Account",
               new { username = username, token = token },
               protocol: Request.Url.Scheme);

            return callbackUrl;
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