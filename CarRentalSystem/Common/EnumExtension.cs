using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRentalSystem.Common
{
    public static class EnumExtension
    {
        /// <summary>
        /// HasElements() check if it's not null and contains elements.
        /// usable on linq statements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myList"></param>
        /// <returns></returns>
        public static bool HasElements<T>(this IEnumerable<T> myList)
        {
            return (myList != null && myList.Count() > 0);
        }

    }
}