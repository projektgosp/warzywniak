using System;
using System.Web.Http.WebHost;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using projekt_gosp.Models;
using projekt_gosp.Helpers;
using WebMatrix.WebData;
using Newtonsoft.Json;

namespace projekt_gosp.Controllers
{
    public class CartController : Controller
    {
        private db context = new db();

        [Authorize]
        public ActionResult Index()
        {
            return View("cart");
        }

        class CartInfo
        {
            public double total { set; get; }
            public List<ProductInfo> productsList { set; get; }
            public CartInfo(List<ProductInfo> list, double cost)
            {
                total = cost;
                productsList = list;
            }
        }

        class ProductInfo
        {
            public string Nazwa { set; get; }
            public decimal Ilosc { set; get; }
            public double Koszt { set; get; }
            public int ID_Towaru { set; get; }

            public ProductInfo(string nazwa, double koszt, decimal ilosc, int id_towaru)
            {
                Nazwa = nazwa;
                Koszt = koszt;
                Ilosc = ilosc;
                ID_Towaru = id_towaru;
            }
        }


        public ActionResult Cart()
        {
            double total = 0;
        
            var productsList = new List<ProductInfo>();
            string username = User.Identity.Name;
            int shopId = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
     
            var cartItems = (from b in context.Koszyk where b.UserName==WebSecurity.CurrentUserName && b.ShopId==shopId select b).ToList();
            // w petli jest Towar towar = context.Towary.Find(cart.ID_towaru); ktore otwiera readera do bazy
            // przez co walnie bledem, gdyz reader byl juz otwarty przy cartItems, majac .ToList() pobieramy natychmiastowo dane i reader sie zamyka

            foreach (var cart in cartItems)
            {
                Towar towar = context.Towary.Find(cart.ID_towaru);
                decimal ItemQty = cart.Ilosc;
                double cena = towar.Cena * (double)ItemQty;
                string nazwa = towar.Produkt.Nazwa;
                
                productsList.Add(new ProductInfo(nazwa, cena, ItemQty, cart.Id));
                total += cena;
            }
            CartInfo cartInfo = new CartInfo(productsList, total);

            return Json(cartInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Info(CartModel cartModel)
        {  
            string username = User.Identity.Name;
            cartModel.UserName = username;
            cartModel.ShopId = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
            context.Koszyk.Add(cartModel);
            context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            CartModel cartModel = context.Koszyk.Find(id);
            context.Koszyk.Remove(cartModel);
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
