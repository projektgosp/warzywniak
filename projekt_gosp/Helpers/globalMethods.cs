using projekt_gosp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projekt_gosp.Helpers
{
    public static class GlobalMethods
    {
        public static int GetShopId(int userid, db context, bool isAuthenticated, HttpSessionStateBase session)
        {
            int shopid = 0;
            if (isAuthenticated)
            {
                shopid = (from p in context.Uzytkownicy
                              where p.ID_klienta == userid
                              select p.selectedShopId).FirstOrDefault();
                if (shopid == 0)
                {
                    shopid = GlobalMethods.GetDefaultShopId(context);
                }
            }
            else
            {
                if (session["shopid"] == null)
                {
                    shopid = GlobalMethods.GetDefaultShopId(context);
                }
                else
                {
                    shopid = Int32.Parse(session["shopid"].ToString());
                }
            }
            return shopid;
        }

        public static int GetDefaultShopId(db context)
        {
            int defaultId = (from p in context.Sklepy
                             select p.ID_sklepu).First();

            return defaultId;
        }
    }
}