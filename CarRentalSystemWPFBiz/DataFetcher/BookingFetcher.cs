using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFBiz.DataLayer;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WPFBiz.DataFetcher
{
    public static class BookingFetcher
    {
        public static List<Booking> GetBookingByCarId(int carId, DataManager manager)
        {
            string url = $"api/Booking?rowIndex=0&rowForPage=1000&carId={carId.ToString()}";
            List<Booking> objEntity = manager.GetMany<Booking>(url);
            if (objEntity != null)
            {
                return objEntity;
            }
            return new List<Booking>();
        }

        public static bool SaveBooking(Booking booking, DataManager manager)
        {
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>());
            HttpClient _client = manager.GetHttpClient();
            HttpResponseMessage response = _client.PostAsync($"/api/Booking?carId={booking.CarId}&customerName={booking.CustomerName}&startDate={ String.Format("{0:s}", booking.StartDate)}&endDate={String.Format("{0:s}", booking.EndDate)}", formContent).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return false;

            return true;
        }

        public static bool UpdateBooking(Booking booking, DataManager manager)
        {
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>());
            HttpClient _client = manager.GetHttpClient();

            //TODO: put method not implemented yet
            HttpResponseMessage response = _client.DeleteAsync($"api/Booking?bookingId={booking.BookingId}").Result;


            response = _client.PostAsync($"/api/Booking?carId={booking.CarId}&customerName={booking.CustomerName}&startDate={ String.Format("{0:s}", booking.StartDate)}&endDate={String.Format("{0:s}", booking.EndDate)}", formContent).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return false;

            return true;
        }

        public static bool DeleteBooking(int bookingId, DataManager manager)
        {
            HttpClient _client =   manager.GetHttpClient();
            HttpResponseMessage resp = _client.DeleteAsync($"api/Booking?bookingId={bookingId.ToString()}").Result;
            resp.EnsureSuccessStatusCode();
            return true;
        }
    }
}
