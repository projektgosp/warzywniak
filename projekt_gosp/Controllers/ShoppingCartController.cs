using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using projekt_gosp.Controllers;
using projekt_gosp.Models;
using WebMatrix.WebData;

namespace projekt_gosp.Controllers
{
    public class ShoppingCartController : Controller
    {
        private db context = new db();
        private Cartdb Cartcontext = new Cartdb();

        //
        // GET: /ShoppingCart/
    //    public ActionResult Index()
    //    {
    //        return View();
    //    }

    //    public int AddCart(int ItemId, int Quantity)
    //    {
    //        string username = Session["username"].ToString();
    //        CartModel objcart = new CartModel()
    //        {
    //            UserName = username,
    //            ID_produktu = ItemId,
    //            Ilosc = Quantity
    //        };

    //        Cartcontext.Koszyk.Add(objcart);
    //        Cartcontext.SaveChanges();
    //        int count = Cartcontext.Koszyk.Where(s => s.UserName == username).Count();
    //        return count;
    //    }

    //    public PartialViewResult GetCartItems()
    //    {
    //        string username = Session["username"].ToString();
    //        double sum = 0;
    //        var GetItemsId = Cartcontext.Koszyk.Where(s => s.UserName == username).Select(u => u.ID_produktu).ToList();
    //        var GetCartItem = from itemList in context.Produkty where GetItemsId.Contains(itemList.ID_produktu) select itemList;
    //        foreach (var totalsum in GetCartItem)
    //        {
    //            int ItemQty = Cartcontext.Koszyk.Where(u => u.ID_produktu == totalsum.ID_produktu).Select(z => z.Ilosc).First();
    //            sum = sum + ( totalsum.Cena * ItemQty );
    //        }
    //        ViewBag.Total = sum;
    //        return PartialView("~/views/ShoppingCart/_cartItem.cshtml", GetCartItem);

    //    }

    //    public PartialViewResult DeleteCart(int itemId)
    //    {
    //        string getName = Session["username"].ToString();
    //        CartModel removeCart = Cartcontext.Koszyk.FirstOrDefault(s => s.UserName == getName && s.ID_produktu == itemId);
    //        Cartcontext.Koszyk.Remove(removeCart);
    //        Cartcontext.SaveChanges();
    //        return GetCartItems();
    //    }

    }
}
