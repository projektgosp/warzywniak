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
    public class ExpireDateController : Controller
    {
        private db context = new db();

        //
        // GET: /ExpireDate/

        public ActionResult Index()
        {
            int shopId = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
            var expireDate = DateTime.Now.Date.AddDays(7);
            var products = (from b in context.Towary
                            where b.Data_waznosci <= expireDate &&
                                b.ID_sklepu == shopId
                            select b).ToList();

            return View(products);
        }
        
        public class Promo {
            public double new_price { set; get; }
            public int id { set; get; }
        }

        [HttpPost]
        public ActionResult AddPromotion(Promo promo)
        {
            if (promo == null || promo.id == 0)
            {
                return Json(new { success = false });
            }
            int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);

            Towar towar = context.Towary.Find(promo.id);
            towar.Cena = promo.new_price;
            if(context.Promocje.Any(o => o.ID_towaru == promo.id && o.ID_sklepu == shopid)){
                Promocja promocja = (from p in context.Promocje
                    where p.ID_sklepu == shopid && p.ID_towaru == promo.id
                    select p).First();
                promocja.cena_promo = Convert.ToDecimal(promo.new_price);
            }
            else 
            {
                Promocja promocja = new Promocja();
                promocja.ID_towaru = promo.id;
                promocja.ID_sklepu = towar.ID_sklepu;
                promocja.Sklep = towar.Sklep;
                promocja.ID_kategorii = towar.Produkt.ID_kategorii;
                promocja.Kategoria = towar.Produkt.Kategoria;
                promocja.cena_promo = Convert.ToDecimal(promo.new_price);
                promocja.Towar = towar;
                context.Promocje.Add(promocja);
            }

            context.SaveChanges();
            return Json(new { success = true });
        }

        public ActionResult Products(int id = 0)
        {
            if(id == 0)
            {
                return View("error");
            }
            int shopId = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
            var expireDate = DateTime.Now.Date.AddDays(id);
            var products = (from b in context.Towary
                            where b.Data_waznosci <= expireDate &&
                                b.ID_sklepu == shopId
                            select b).ToList();

            return View(products);
        }

        public ActionResult SelectDayToExpire()
        {
            return View();
        }

    }
}
