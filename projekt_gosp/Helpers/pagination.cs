using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projekt_gosp.Helpers
{
    public static class pagination
    {
        public static int pageSize = 10;
        public static int visibleNumbers = 9; // this variable must be odd!

        /*
         * List<int>[0] = pagesCount
         * List<int>[1] = start point
         * List<int>[2] = current page
         * List<int>[3] = end point
         */
        public static List<int> calculatePagination(int page, int itemsCount)
        {
            List<int> outcome = new List<int>(4);
            int visNumbers = pagination.visibleNumbers;
            int pagesCount = (int)Math.Ceiling(itemsCount / (double)pagination.pageSize);

            outcome.Add(pagesCount);
            if (pagesCount <= 1)
            {
                outcome.Add(0);
                outcome.Add(0);
                outcome.Add(-1);
                return outcome;
            }

            int center = 0;

            if (pagesCount < visNumbers)
            {
                visNumbers = pagesCount;
            }

            center = visNumbers / 2;

            if (page > center) // proba wysrodkowania
            {
                if (pagesCount - page >= center) // gdy zakres stron da sie jeszcze "przewijac"
                {
                    outcome.Add(page - center);
                    outcome.Add(page);
                    outcome.Add(page + center);
                }
                else // gdy zakres sie skonczyl to dochodzimy do jego konca
                {
                    outcome.Add(pagesCount - visNumbers + 1);
                    outcome.Add(page);
                    outcome.Add(pagesCount);
                }
            }
            else
            {
                outcome.Add(1);
                outcome.Add(page);
                outcome.Add(visNumbers);
            }

            return outcome;
        }
    }
}