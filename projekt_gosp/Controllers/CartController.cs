using System;
using System.Web.Http.WebHost;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projekt_gosp.Controllers
{
    public class CartController : Controller
    {

        // GET: /api/cart

        class CartInfo
        {
            public string Nazwa {get; set;}
            public float Ilosc {get; set;}
            public float Koszt {get; set;}
        };
        
        public ActionResult Cart()
        {
            var products = new List<CartInfo>();
            float total = 0;

            products.Add(new CartInfo(){ Nazwa = "Ziemniak", Ilosc = 1, Koszt = 2});
            products.Add(new CartInfo(){ Nazwa = "Ananas", Ilosc = 2, Koszt = 5 });

            foreach (var product in products)
            {
                total += product.Koszt;
            }

            products.Add(new CartInfo() {Nazwa = "Suma", Ilosc = 0, Koszt = total});

            return Json(products, JsonRequestBehavior.AllowGet);
        }

    }
}
