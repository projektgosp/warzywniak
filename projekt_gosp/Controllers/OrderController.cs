using projekt_gosp.Helpers;
using projekt_gosp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace projekt_gosp.Controllers
{
    [Authorize(Roles = "customer")]
    public class OrderController : Controller
    {
        private db context = new db();

        [HttpGet]
        public ActionResult CreateOrder()
        {
            List<CartModel> itemsFromCart = GetItemsFromCart();

            int shopId = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);

            if(itemsFromCart == null)
            {
                return HttpNotFound();
            }

            List<additionalModels.OrderModel> ViewOrder = GetOrderModelFromCart(itemsFromCart);
            ViewOrder.RemoveAll(x => x.quantity < 0);
            double totalSum = CalculateItemsPrice(ViewOrder);

            var order = new Zamowienie()
            {
                czyPotwierdzonePrzezKlienta = false,
                Data_zam = DateTime.Now,
                ID_klienta = WebSecurity.CurrentUserId,
                ID_sklepu = shopId,
                kwotaZamowienia = totalSum
            };

            context.Zamowienia.Add(order);
            context.SaveChanges();

            foreach (var item in ViewOrder)
            {
                if (item.quantity > 0)
                {
                    context.Pozycje_zamowienia.Add(new Pozycja_zamowienia()
                    {
                        ID_Towaru = item.merchendiseId,
                        ID_zamowienia = order.ID_zamowienia,
                        Ilosc = item.quantity
                    });
                }
            }

            order.kwotaZamowienia = totalSum;

            context.SaveChanges();

            ViewBag.totalSum = totalSum;
            ViewBag.orderID = order.ID_zamowienia;
            return View(ViewOrder);
        }

        [HttpGet]
        public ActionResult ConfirmOrder(int id = 0, string pay="money")
        {
            if(id < 1)
            {
                return HttpNotFound();
            }

            var order = (from p in context.Zamowienia
                         where p.ID_zamowienia == id && p.ID_klienta == WebSecurity.CurrentUserId
                         select p).FirstOrDefault();

            foreach(var item in order.Pozycje_zamowienia)
            {
                if (item.Towar.Ilosc - item.Ilosc >= 0 && item.Ilosc > 0)
                {
                    item.Towar.Ilosc -= item.Ilosc;
                }
                else
                {
                    ViewBag.itemName = item.Towar.Produkt.Nazwa;
                    return View("ordererror");
                }
            }

            order.czyPotwierdzonePrzezKlienta = true;

            if (pay == "money")
            {
                calculatePoints(order.kwotaZamowienia);
            }
            else if (pay == "points")
            {
                if(!PaymantByPoints(order.kwotaZamowienia))
                {
                    return View("notEnoughPoints");
                }
            }
            context.SaveChanges();

            try
            {
                var user = (from p in context.Uzytkownicy
                            where p.ID_klienta == WebSecurity.CurrentUserId
                            select p).FirstOrDefault();


                var subject = "Grocery shop - zamowienie";
                var body = String.Format("Zamówienie zostało złożone.");
                RemoveItemsFromCart();
                GlobalMethods.SendMailThread(user.Email, subject, body);

                string clientPhoneNumber = order.Klient.Nr_tel;
                string orderValue = order.kwotaZamowienia.ToString();

                string message = "Witaj! Twoje zamówienie w sklepie e-Warzywko na kwotę w wysokości " + orderValue + " zł zostało przyjęte do realizacji.";

                GlobalMethods.SendSmsToClientThread(clientPhoneNumber, message);
            }
            catch
            {
                return View();
            }

            return View();

        }

        private bool PaymantByPoints(double cost)
        {
            var user = (from p in context.Uzytkownicy
                        where p.ID_klienta == WebSecurity.CurrentUserId
                        select p).FirstOrDefault();
            int neededPoints = Convert.ToInt32(cost);
            if (neededPoints <= user.Punkty)
            {
                user.Punkty -= neededPoints;
            }
            else
            {
                return false;
            }
            return true;
        }

        private bool RemoveItemsFromCart()
        {
            List<CartModel> itemsFromCart = (from p in context.Koszyk
                                             where p.UserName == WebSecurity.CurrentUserName
                                             select p).ToList();

            foreach (var cartItem in itemsFromCart)
            {
                context.Koszyk.Remove(cartItem);
            }
            context.SaveChanges();
            
            return true;
        }

        private List<CartModel> GetItemsFromCart()
        {
            List<CartModel> itemsFromCart = (from p in context.Koszyk
                                             where p.UserName == WebSecurity.CurrentUserName
                                             select p).ToList();

            return itemsFromCart;
        }

        private List<additionalModels.OrderModel> GetOrderModelFromCart(List<CartModel> cartItems)
        {
            List<additionalModels.OrderModel> order = new List<additionalModels.OrderModel>();

            List<int> merchandiseId = GetMerchandiseIDList(cartItems);

            List<Towar> merchandises = GetMerchandiseList(merchandiseId);

            if (merchandises == null)
            {
                return null;
            }

            foreach (var cartItem in cartItems)
            {
                foreach (var merchendise in merchandises)
                {
                    if (merchendise.ID_Towaru == cartItem.ID_towaru)
                    {
                        if(cartItem.Ilosc > merchendise.Ilosc)
                        {
                            cartItem.Ilosc = merchendise.Ilosc;
                        }

                        order.Add(new additionalModels.OrderModel
                        {
                            merchendiseId = merchendise.ID_Towaru,
                            itemName = merchendise.Produkt.Nazwa,
                            price = merchendise.Cena,
                            quantity = cartItem.Ilosc
                        });
                    }
                }
            }

            return order;
        }

        private List<int> GetMerchandiseIDList(List<CartModel> cartItems)
        {
            List<int> merchandiseIdList = new List<int>();
            foreach (var cartItem in cartItems)
            {
                merchandiseIdList.Add(cartItem.ID_towaru);
            }

            return merchandiseIdList;
        }

        private List<Towar> GetMerchandiseList(List<int> merchandiseIdList)
        {
            var merchandises = (from p in context.Towary.Include("Produkt")
                                where merchandiseIdList.Contains(p.ID_Towaru)
                                select p).ToList();

            return merchandises;
        }

        private double CalculateItemsPrice(List<additionalModels.OrderModel> order)
        {
            double price = 0;

            foreach(var item in order)
            {
                price += ((double)item.quantity * item.price);
            }

            return price;
        }

        // naliczanie punktow
        private void calculatePoints(double money)
        {
            //todo
            var user = (from p in context.Uzytkownicy
                        where p.ID_klienta == WebSecurity.CurrentUserId
                        select p).FirstOrDefault();
            int points = 0;
            if (money > 10)
            {
                points = 2 * (Convert.ToInt32(money) / 10);
            }
            user.Punkty += points;
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
