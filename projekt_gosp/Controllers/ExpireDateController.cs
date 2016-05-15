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
            var products = (from b in context.Towary where b.Data_waznosci <= expireDate &&
                                 b.ID_sklepu == shopId select b).ToList();
   
            return View(products);
        }

    }
}
