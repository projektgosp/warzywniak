using projekt_gosp.Models;
using projekt_gosp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Threading;

namespace projekt_gosp.Controllers
{
    [Authorize(Roles = "shop")]
    public class SellerPanelController : Controller
    {
        private db context = new db();
        //
        // GET: /ShopPanel/

        public ActionResult page(int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
            var items = (from p in context.Towary
                         where p.ID_sklepu == shopid
                         select p).ToList();

            int itemsCount = items.Count();

            List<int> calculatedPagination = pagination.calculatePagination(page, itemsCount);

            ViewBag.pagesCount = calculatedPagination[0];
            ViewBag.startPage = calculatedPagination[1];
            ViewBag.activePage = calculatedPagination[2];
            ViewBag.endPage = calculatedPagination[3];

            return View("index",items);
        }

        [HttpGet]
        public ActionResult addItemToShop(int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            int itemsCount = context.Produkty.Count();

            List<int> calculatedPagination = pagination.calculatePagination(page, itemsCount);

            ViewBag.pagesCount = calculatedPagination[0];
            ViewBag.startPage = calculatedPagination[1];
            ViewBag.activePage = calculatedPagination[2];
            ViewBag.endPage = calculatedPagination[3];

            var products = (from p in context.Produkty
                            select p).ToList();

            return View("globalProductList", products);
        }

        [HttpGet]
        public ActionResult addItem(int id = 0)
        {
            if (id != 0)
            {
                var item = (from p in context.Produkty
                            where p.ID_produktu == id
                            select p).FirstOrDefault();

                if (item != null)
                {
                    return View(item);
                }
            }

            return View("Error");
        }

        [HttpPost]
        public ActionResult addItem(Produkt product, DateTime expiryDate = default(DateTime), int count = -1, int id = 0)
        {
            if (ModelState.IsValid && expiryDate != default(DateTime) && count > 0 && id > 0) 
            {
                int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
                if(shopid < 0)
                {
                    return View("Error");
                }

                var prod = (from p in context.Produkty
                               where p.ID_produktu == id
                               select p).FirstOrDefault();
                if (prod == null)
                {
                    return View("Error");
                }

                Towar t = new Towar
                {
                    Data_waznosci = expiryDate,
                    ID_produktu = id,
                    ID_sklepu = shopid,
                    Ilosc = count,
                    Cena = product.Cena
                };

                context.Towary.Add(t);
                context.SaveChanges();

                return RedirectToAction("addItemToShop", "sellerpanel");
            }

            ViewBag.error = "Wszystkie pola sa wymagane!";
            var item = (from p in context.Produkty
                        where p.ID_produktu == id
                        select p).FirstOrDefault();

            return View(item);
        }

        //edit towaru, nie produktu!!
        [HttpGet]
        public ActionResult editItem(int id = 0)
        {
            if (id != 0)
            {
                int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
                var item = (from p in context.Towary
                            where p.ID_Towaru == id && p.ID_sklepu == shopid
                            select p).FirstOrDefault();

                if (item != null)
                {
                    return View(item);
                }
            }

            return View("Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editItem(Towar item, int id = 0)
        {
            if (id <= 0)
            {
                return Json(new { error = "Brak wyników wyszukiwania" });
            }

            if (ModelState.IsValid)
            {
                int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
                var itemToEdit = (from p in context.Towary
                                  where p.ID_Towaru == id && p.ID_sklepu == shopid
                                  select p).FirstOrDefault();
                if (itemToEdit == null)
                {
                    return Json(new { error = "Brak wyników wyszukiwania" });
                }

                itemToEdit.Ilosc = item.Ilosc;
                itemToEdit.Cena = item.Cena;
                itemToEdit.Data_waznosci = item.Data_waznosci;


                context.SaveChanges();
                return RedirectToAction("page", "sellerpanel");
            }

            return Json(new { error = "Wszystkie pola są wymagane!" });
        }

        [HttpGet]
        public ActionResult removeItem(int id = 0)
        {
            int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
            var item = (from p in context.Towary
                        where p.ID_Towaru == id && p.ID_sklepu == shopid
                        select p).FirstOrDefault();

            if (item != null)
            {
                context.Towary.Remove(item);
                context.SaveChanges();
            }
            return RedirectToAction("page");
        }

        public ActionResult settings()
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
                return Json(new { success = "0", error = "Pole e-mail nie może być puste" }, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public ActionResult ShopOrders()
        {
            int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
            List<Zamowienie> orders = (from p in context.Zamowienia
                                       where p.ID_sklepu == shopid && p.czyPotwierdzonePrzezKlienta == true
                                       orderby p.ID_zamowienia descending
                                       select p).ToList();
            return View(orders);
        }

        public ActionResult ChangeOrderStatus(int id = 0)
        {
            if (id < 1)
            {
                return HttpNotFound();
            }

            int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
            var order = (from p in context.Zamowienia
                         where p.ID_zamowienia == id && p.ID_sklepu == shopid
                         select p).FirstOrDefault();
            if (order.statusZamowienia == false)
            {
                order.statusZamowienia = true;

                string clientPhoneNumber = order.Klient.Nr_tel;
                string shopAddress = "ulica " + order.Sklep.Adres.Ulica + " " + order.Sklep.Adres.Nr_budynku;
                string orderValue = order.kwotaZamowienia.ToString();

                string message = "Witaj! Twoje zamówienie na kwotę w wysokości " + orderValue + " zł wykonane w sklepie e-Warzywko jest już gotowe do odbioru. Zapraszamy po odbiór pod adresem: " + shopAddress;


                //NIE RUSZAC BO LIMITY DARMOWYCH SMSOW MAMY
                //DZIALAC - DZIALA
                //GlobalMethods.SendSmsToClientThread(clientPhoneNumber, message);
            }

            context.SaveChanges();

            return Json(new { success = "1" });
        }

        public ActionResult removeconfirmedorder(int id = 0, string reason = "")
        {
            if (reason == "" || id == 0)
            {
                return Json(new { success = false });
            }

            int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);

            Zamowienie order = (from p in context.Zamowienia
                                where p.ID_zamowienia == id && p.ID_sklepu == shopid
                                select p).FirstOrDefault();

            if (order == null)
            {
                return Json(new { success = false });
            }

            foreach (var orderItem in order.Pozycje_zamowienia)
            {
                orderItem.Towar.Ilosc += orderItem.Ilosc;
                //context.Pozycje_zamowienia.Remove(orderItem);
            }

            //order.Pozycje_zamowienia.ToList()

            string phoneNumber = order.Klient.Nr_tel;
            string orderValue = order.kwotaZamowienia.ToString();
            string message = "Witaj! Niestety, ale Twoje zamówienie w sklepie e-Warzywko na kwotę w wysokości " + orderValue + " zł zostało usunięte. Powod: " + reason;

            //NIE RUSZAC BO LIMITY DARMOWYCH SMSOW MAMY
            //DZIALAC - DZIALA
            //GlobalMethods.SendSmsToClientThread(phoneNumber, message);

            context.Zamowienia.Remove(order);

            context.SaveChanges();

            return Json(new { success = true });
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
