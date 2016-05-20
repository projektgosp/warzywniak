using projekt_gosp.Models;
using projekt_gosp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace projekt_gosp.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminPanelController : Controller
    {
        private db context = new db();

        public ActionResult page(int id = 1)
        {
            int page = id;
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

            var items = (from p in context.Produkty
                         orderby p.ID_produktu descending
                         select p).Skip((page - 1) * pagination.pageSize).Take(pagination.pageSize).ToList();

            return View("index", items);
        }

        [HttpGet]
        public ActionResult addCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult addCategory(string name = null)
        {
            if (name != null)
            {
                string nameToLink = name.Replace(' ', '-');

                context.Kategorie.Add(new Kategoria
                {
                    NameToLink = nameToLink,
                    NameToDisplay = name
                });

                context.SaveChanges();
            }

            return View();
        }
        
        [HttpGet]
        public ActionResult addItem()
        {
            ViewBag.categories = (from p in context.Kategorie
                                  select p).ToList();

            ViewBag.priceTypes = (from p in context.RodzajeCeny
                                  select p).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult addItem(Produkt item)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    item.attachedImage = true;
                }

                if (Request.Files.Count == 1)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var path = fileHelper.addfile(file);
                        item.fullSizePath = path[0];
                        item.thumbPath = path[1];
                    }
                }

                item.Data_dodania = DateTime.Now;

                context.Produkty.Add(item);
                context.SaveChanges();

                return RedirectToAction("globalid", "items", new { id = item.ID_produktu });
            }

            ViewBag.categories = (from p in context.Kategorie
                                  select p).ToList();

            ViewBag.priceTypes = (from p in context.RodzajeCeny
                                  select p).ToList();

            return View();
        }
        
        [HttpGet]
        public ActionResult editItem(int id = 0)
        {
            if (id < 1)
            {
                return View("Error");
            }

            ViewBag.attachedImage = false;

            var item = (from p in context.Produkty
                        where p.ID_produktu == id
                        select p).FirstOrDefault();

            if (item == null)
            {
                return View("Error");
            }


            ViewBag.categories = (from p in context.Kategorie
                                  select p).ToList();

            ViewBag.priceTypes = (from p in context.RodzajeCeny
                                  select p).ToList();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editItem(Produkt editedItem, int id = 0, bool removeFile = false)
        {
            if (id <= 0)
            {
                return Json(new { error = "Brak wyników wyszukiwania" });
            }

            if (ModelState.IsValid)
            {

                var itemToEdit = (from p in context.Produkty
                                  where p.ID_produktu == id
                                  select p).FirstOrDefault();

                if (itemToEdit == null)
                {
                    return Json(new { error = "Brak wyników wyszukiwania" });
                }

                itemToEdit.Nazwa = editedItem.Nazwa;
                itemToEdit.Opis = editedItem.Opis;
                itemToEdit.Cena = editedItem.Cena;
                itemToEdit.ID_kategorii = editedItem.ID_kategorii;
                itemToEdit.ID_RodzajuCeny = editedItem.ID_RodzajuCeny;

                if (itemToEdit.attachedImage == true && removeFile == true)
                {
                    fileHelper.deletefile(itemToEdit.thumbPath);
                    fileHelper.deletefile(itemToEdit.fullSizePath);
                    itemToEdit.attachedImage = false;
                }

                if (Request.Files.Count == 1 && itemToEdit.attachedImage == false)
                {
                    itemToEdit.attachedImage = true;

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];
                        if (file != null && file.ContentLength > 0)
                        {
                            var path = fileHelper.addfile(file);
                            itemToEdit.fullSizePath = path[0];
                            itemToEdit.thumbPath = path[1];
                        }
                    }
                }

                context.SaveChanges();
                return RedirectToAction("globalid", "items", new { id = itemToEdit.ID_produktu });
            }

            return Json(new { error = "Wszystkie pola są wymagane." });
        }

        [HttpGet]
        public ActionResult removeItem(int id = 0)
        {
            var item = (from p in context.Produkty
                        where p.ID_produktu == id
                        select p).FirstOrDefault();
            if (item != null)
            {
                if (item.attachedImage == true)
                {
                    fileHelper.deletefile(item.thumbPath);
                    fileHelper.deletefile(item.fullSizePath);
                }

                context.Produkty.Remove(item);

                context.SaveChanges();
            }
            return RedirectToAction("index");
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
