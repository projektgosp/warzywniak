using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using projekt_gosp.Models;
using projekt_gosp.Helpers;
using WebMatrix.WebData;

namespace projekt_gosp.Controllers
{
    public class ShopController : Controller
    {
        private db context = new db();
        //
        // GET: /Shop/

        public ActionResult Index()
        {
            int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
            var promotions = (from p in context.Promocje
                              where p.ID_sklepu == shopid
                              select p).ToList();


            var newItems = (from p in context.Towary
                            where p.ID_sklepu == shopid
                            orderby p.ID_Towaru descending
                            select p).Take(6).ToList();

            ViewBag.newItems = newItems;

            return View(promotions);
        }

        private List<additionalModels.shopDisplayModel> getShopList()
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
                    flatNumber = shop.Adres.Nr_lokalu,
                    isSelected = false
                });
            }

            return shopsList;
        }


        public ActionResult selectShop(int id = 0)
        {
            if (id > 0 && !User.IsInRole("admin") && !User.IsInRole("shop"))
            {
                var shop = (from p in context.Sklepy
                            where p.ID_sklepu == id
                            select p).FirstOrDefault();
                if (shop != null)
                {
                    if (WebSecurity.IsAuthenticated)
                    {
                        var user = (from p in context.Uzytkownicy
                                    where p.ID_klienta == WebSecurity.CurrentUserId
                                    select p).FirstOrDefault();

                        user.selectedShopId = id;
                        context.SaveChanges();
                    }
                    else
                    {
                        Session["shopid"] = id;
                    }

                    return Json(new { success = "1" });
                }
            }

            return View("error");
        }

        public ActionResult getSelectedShop()
        {
            int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);

            var shops = getShopList();

            var selectedShop = shops.Find(x => x.shopID == shopid);
            if (selectedShop != null)
            {
                selectedShop.isSelected = true;
            }
            return PartialView(shops);
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
