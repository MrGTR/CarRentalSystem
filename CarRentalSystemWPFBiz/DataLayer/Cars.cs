using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WPFBiz.DataLayer
{
    public class Car
    {
        public int CarId { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal PriceByDay { get; set; }
        public override string ToString()
        {
            return $"Plate = {CarId.ToString()} - Name = {Name} - Brand = {Brand}";
        }
    }
}
