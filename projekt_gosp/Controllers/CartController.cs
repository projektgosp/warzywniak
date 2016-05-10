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
        private Cartdb Cartcontext = new Cartdb();

        //
        // GET: /ShoppingCart/
        public ActionResult Index()
        {
            return View();
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
            public int Ilosc { set; get; }
            public double Koszt { set; get; }
            public int ID_Towaru { set; get; }

            public ProductInfo(string nazwa, double koszt, int ilosc, int id_towaru) {
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
     
            var cartItems = from b in Cartcontext.Koszyk where b.UserName=="torbiar" && b.ShopId==shopId select b;

            foreach (var cart in cartItems)
            {
                Towar towar = context.Towary.Find(cart.ID_towaru);
                int ItemQty = cart.Ilosc;
                double cena = towar.Cena * ItemQty;
                string nazwa = towar.Produkt.Nazwa;
                
                productsList.Add(new ProductInfo(nazwa, cena, ItemQty, cart.Id));
                total += cena;
            }
            CartInfo cartInfo = new CartInfo(productsList, total);

            return Json(cartInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Info(CartModel cartModel)
        {  
            string username = User.Identity.Name;
            cartModel.UserName = username;
            cartModel.ShopId = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
            Cartcontext.Koszyk.Add(cartModel);
            Cartcontext.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            CartModel cartModel = Cartcontext.Koszyk.Find(id);
            Cartcontext.Koszyk.Remove(cartModel);
            Cartcontext.SaveChanges();
            return Json(new { success = true });
        }

    }
}
