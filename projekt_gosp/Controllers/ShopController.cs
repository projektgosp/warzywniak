using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using projekt_gosp.Models;
using projekt_gosp.Helpers;

namespace projekt_gosp.Controllers
{
    public class ShopController : Controller
    {
        private db context = new db();
        //
        // GET: /Shop/

        public ActionResult Index()
        {
            var promotions = (from p in context.Promocje
                          select p).ToList();


            var newItems = (from p in context.Produkty
                            orderby p.Data_dodania descending
                            select p).Take(6).ToList();

            ViewBag.newItems = newItems;

            return View(promotions);
        }

        [HttpGet]
        public ActionResult selectShop()
        {
            var shops = (from p in context.Sklepy
                         select p).ToList();

            List<additionalModels.shopDisplayModel> shopsList = new List<additionalModels.shopDisplayModel>();

            foreach(var shop in shops)
            {
                shopsList.Add(new additionalModels.shopDisplayModel
                {
                    shopID = shop.ID_sklepu,
                    email = shop.Email,
                    phoneNumber = shop.Nr_tel,
                    city = shop.Adres.Miasto,
                    postalCode = shop.Adres.Kod_pocztowy,
                    street = shop.Adres.Ulica,
                    streetNumber = shop.Adres.Nr_budynku,
                    flatNumber = shop.Adres.Nr_lokalu
                });
            }

            return View(shopsList);
        }

        public ActionResult categories()
        {
            var categories = (from p in context.Kategorie
                              select p).ToList();
            return PartialView(categories);

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
