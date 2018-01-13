using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRentalSystem.Common
{
    public static class ExceptionExtension
    {
        /// <summary>
        /// Prepare complete exception message
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>s
        public static string ParseEx(this Exception ex)
        {
            string strRetValue = ex.Message;
            strRetValue += ex.InnerException != null ? Environment.NewLine + ParseEx(ex.InnerException) : Environment.NewLine + ex.StackTrace;
            return strRetValue;
        }
    }
}