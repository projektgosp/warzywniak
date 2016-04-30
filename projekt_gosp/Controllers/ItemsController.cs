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
    public class ItemsController : Controller
    {
        private db context = new db();
        //
        // GET: /Items/

        public ActionResult Index()
        {
            return View();
        }

        //id towaru
        [Authorize(Roles = "shop, customer")]
        [HttpGet]
        public ActionResult id(int id = 0)
        {
            Towar product = context.Towary.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //id produktu
        [Authorize(Roles = "admin, shop")]
        [HttpGet]
        public ActionResult globalid(int id = 0)
        {
            Produkt product = context.Produkty.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpGet]
        public ActionResult category(string category = null, int page = 1)
        {
            if (category == null)
            {
                ViewBag.categoryName = "";
                return View("itemsList");
            }

            if (page < 1)
            {
                page = 1;
            }

            var cat = (from p in context.Kategorie
                       where p.NameToLink == category
                       select p).FirstOrDefault();

            if (cat == null)
            {
                ViewBag.categoryName = "";
                return View("itemsList");
            }

            if(User.IsInRole("admin"))
            {
                int itemsCount = cat.Produkty.Count();
                List<int> calculatedPagination = pagination.calculatePagination(page, itemsCount);

                ViewBag.pagesCount = calculatedPagination[0];
                ViewBag.startPage = calculatedPagination[1];
                ViewBag.activePage = calculatedPagination[2];
                ViewBag.endPage = calculatedPagination[3];
                ViewBag.categoryName = category;

                var items = (from p in context.Produkty
                             orderby p.ID_produktu descending
                             where p.Kategoria.NameToLink == category
                             select p).Skip((page - 1) * pagination.pageSize).Take(pagination.pageSize).ToList();
                return View("globalitemsList", items);
            }
            else
            {

                int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);


                if (shopid == 0)
                {
                    return View("error");
                }

                var catItems = (from p in context.Towary
                                orderby p.ID_Towaru descending
                                where p.Produkt.Kategoria.NameToLink == category && p.ID_sklepu == shopid
                                select p).ToList();

                int itemsCount = catItems.Count();
                List<int> calculatedPagination = pagination.calculatePagination(page, itemsCount);

                ViewBag.pagesCount = calculatedPagination[0];
                ViewBag.startPage = calculatedPagination[1];
                ViewBag.activePage = calculatedPagination[2];
                ViewBag.endPage = calculatedPagination[3];
                ViewBag.categoryName = category;

                var items = catItems.Skip((page - 1) * pagination.pageSize).Take(pagination.pageSize).ToList();
                return View("itemsList", items);
            }

        }


        public ActionResult SearchItem(string pattern)
        {
            if (User.IsInRole("admin"))
            {
                var products = (from p in context.Produkty
                                where p.Nazwa.ToLower().Contains(pattern.ToLower()) ||
                                      p.Opis.ToLower().Contains(pattern.ToLower())
                                select p).ToList();

                return View("globalsearchitem", products);
            }
            else
            {
                int shopid = GlobalMethods.GetShopId(WebSecurity.CurrentUserId, context, WebSecurity.IsAuthenticated, Session);
                var products = (from p in context.Towary
                                where (p.Produkt.Nazwa.ToLower().Contains(pattern.ToLower()) ||
                                      p.Produkt.Opis.ToLower().Contains(pattern.ToLower())) && p.ID_sklepu == shopid
                                select p).ToList();

                return View("searchitem", products);
            }
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
