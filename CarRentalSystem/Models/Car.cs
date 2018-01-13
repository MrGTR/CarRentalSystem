using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CarRentalSystem.Models
{
    public class Car
    {
        public int CarId { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal PriceByDay { get; set; }
        public bool IsObsolete { get; set; }

        public override string ToString()
        {
           return $"{nameof(CarId)}={CarId} {nameof(Name)}={Name} {nameof(Brand)}={Brand}";
        }

    }

}