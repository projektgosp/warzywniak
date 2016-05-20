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
                                ViewBag.error = "Sprawdź e-mail, aby aktywować konto.";
                            }
                            else
                            {
                                ViewBag.error = "Nazwa użytkownika lub hasło jest błędne!";
                            }
                        }
                        else
                        {
                            ViewBag.error = "Nazwa użytkownika lub hasło jest błędne!";
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
                            Nr_tel = user.phonenumber,
                            Imie = user.name,
                            Nazwisko = user.surname
                        }, true);

                        string callbackUrl = CreateConfirmUrl(user.username, confirmToken);

                        var subject = "Witamy w e-Warzywko";
                        var body = String.Format("Witaj {0}, potwierdź chęć rejestracji poprzez kliknięcie w link: <a href=\"{1}\">Potwierdź</a>", user.username, callbackUrl);

                        GlobalMethods.SendMailThread(user.email, subject, body);

                        ViewBag.error = "Dziękujemy za rejestrację. Wiadomość potwierdzająca została wysłana na podany adres e-mail: " + user.email +
                            " Prosimy o sprawdzenie skrzynki e-mailowej w celu ukończenia rejestracji.";
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
                    ViewBag.msg = string.Format("Twoje konto jest już aktywne!</br> Wciśnij ten przycik, aby się na nie zalogować : <a href='{0}'>Login</a>", loginUrl);
                }
                else
                {
                    if (WebSecurity.ConfirmAccount(username, token))
                    {
                        var loginUrl = Url.Action(
                            "login", "Account");
                        ViewBag.msg = string.Format("Twoje konto jest już aktywne!</br> Wciśnij ten przycik, aby się na nie zalogować <a href='{0}'>Login</a>", loginUrl);
                    }
                    else
                    {
                        ViewBag.msg = "Wystąpił problem podczas aktywacji Twojego konta.";
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

        [HttpGet]
        public ActionResult forgotPassword()
        {
            if (WebSecurity.IsAuthenticated == false)
            {
                return View();
            }
            return RedirectToAction("Index", "Shop");
        }

        [HttpPost]
        public ActionResult forgotPassword(string username = "", string email = "")
        {
            if (WebSecurity.IsAuthenticated == false)
            {
                bool error = false;
                if (username == "")
                {
                    error = true;
                    ViewBag.error += "pole nazwa nie może być puste</br>";
                }
                if (email == "")
                {
                    error = true;
                    ViewBag.error += "pole email nie może być puste";
                }

                if (error == true)
                {
                    return View();
                }

                var usr = from p in context.Uzytkownicy
                          where p.Email == email && p.accountName == username
                          select p;

                var user = usr.FirstOrDefault();
                if (user == null)
                {
                    ViewBag.error = "Ten email nie pasuje do podanej nazwy użytkownika";
                    return View();
                }
                else
                {
                    string passwordResetToken = WebSecurity.GeneratePasswordResetToken(username);

                    var callbackUrl = Url.Action(
                        "confirmResetPassword", "Account",
                        new { token = passwordResetToken },
                        protocol: Request.Url.Scheme);

                    var subject = "e-Warzywko: resetowanie hasła";
                    var body = String.Format("Witaj {0}, proszę kliknąć w link aby zresetować hasło: <a href=\"{1}\">Resetuj</a>. </br> Jeśli nie zleciłeś zmiany hasła, zignoruj tą wiadomość!", username, callbackUrl);

                    GlobalMethods.SendMailThread(email, subject, body);

                    ViewBag.msg = "Link do zmiany hasła został wysłany na twój adres email";
                    return View();
                }
            }
            return RedirectToAction("Index", "shop");
        }

        [HttpGet]
        public ActionResult confirmResetPassword(string token = "")
        {
            if (WebSecurity.IsAuthenticated == false)
            {
                ViewBag.token = token;
                return View();
            }
            return RedirectToAction("Index", "shop");
        }

        [HttpPost]
        public ActionResult confirmResetPassword(string token = "", string password = "")
        {
            if (WebSecurity.IsAuthenticated == false)
            {
                if (token == "")
                {
                    return RedirectToAction("login", "account");
                }
                if (password == "")
                {
                    ViewBag.error = "pole hasło jest wymagane";
                    return View();
                }

                if (WebSecurity.ResetPassword(token, password))
                {
                    ViewBag.msg = "Hasło zostało zresetowane! Teraz możesz się zalogować";
                }
                else
                {
                    ViewBag.msg = "Ups! Coś poszło nie tak...";
                }
                return View();
            }
            return RedirectToAction("Index", "shop");
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