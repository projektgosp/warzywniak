using projekt_gosp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projekt_gosp.Controllers
{
    public class OrderController : Controller
    {
        private db context = new db();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Checkout()
        {
            return View();
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
