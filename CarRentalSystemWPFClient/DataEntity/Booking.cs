using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystemWPFClient
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int CarId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CustomerName { get; set; }

        public override string ToString()
        {
            return $"Reservation Number = {BookingId}, Customer = {CustomerName}";
        }
    }
}
