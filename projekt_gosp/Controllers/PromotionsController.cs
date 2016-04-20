using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projekt_gosp.Controllers
{
    public class PromotionsController : Controller
    {
        //
        // GET: /Promotions/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult id(int id = 0)
        {
            return null;
        }
    }
}
