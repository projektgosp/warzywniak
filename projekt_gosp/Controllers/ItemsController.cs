using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using projekt_gosp.Models;
using projekt_gosp.Helpers;

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

        [HttpGet]
        public ActionResult id(int id = 0)
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

            int itemsCount = cat.Produkty.Count();

            List<int> calculatedPagination = pagination.calculatePagination(page, itemsCount);

            ViewBag.pagesCount = calculatedPagination[0];
            ViewBag.startPage = calculatedPagination[1];
            ViewBag.activePage = calculatedPagination[2];
            ViewBag.endPage = calculatedPagination[3];

            var items = (from p in context.Produkty
                         orderby p.ID_produktu descending
                         where p.Kategoria.NameToLink == category
                         select p).Skip((page - 1) * pagination.pageSize).Take(pagination.pageSize).ToList();

            ViewBag.categoryName = category;
            return View("itemsList", items);
        }


        public ActionResult SearchItem(string pattern)
        {
            var products = (from p in context.Produkty
                            where p.Nazwa.ToLower().Contains(pattern.ToLower()) ||
                                  p.Opis.ToLower().Contains(pattern.ToLower())
                            select p).ToList();

            return View(products);
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
