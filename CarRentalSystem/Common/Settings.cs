using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRentalSystem.Common
{
    public static class Constants
    {
        static public class AzureDatabase
        {
            private const string sqlInstanceName = "gtr.database.windows.net";
            private const string dBName = "CarRentalSystem";
            private const string userName = "gabriele.tronchin";
            private const string pass = "Montell0";
            public static string ConnectionString = $"Data Source={sqlInstanceName};Initial Catalog={dBName};Integrated Security=False;User ID={userName};Password={pass};Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        }
    }
}