using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CarRentalSystemWPFClient
{
    /// <summary>
    /// Interaction logic for BookingPage.xaml
    /// </summary>
    public partial class BookingPage : Window
    {
        #region "Fields"
        private Car _car;
        private static HttpClient _client = new HttpClient();
        #endregion

        #region "Costructor"
        public BookingPage(Car objCar, HttpClient client)
        {
            _car = objCar;
            _client = client;

            Task<List<Booking>> tBooking =  RefreshBookingListAsync();

            InitializeComponent();

            lblInfo.Content = $"Model = {objCar.Name} - Brand = {objCar.Brand}";
            dtStartDate.SelectedDate = DateTime.Today;
            dtEndDate.SelectedDate = DateTime.Today.AddDays(1);
            txtResult.Visibility = Visibility.Collapsed;

            dgBooking.ItemsSource = tBooking.Result;
        }

        #endregion

        #region "Graphis Events"
        private void Book_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtCustomerName.Text))
                    throw new Exception("Customer Name it's empty");

                if (dtStartDate.SelectedDate == null)
                    throw new Exception("Start Date it's not selected");

                if (dtEndDate.SelectedDate == null)
                    throw new Exception("End Date it's not selected");


                string srStartDate = String.Format("{0:s}", dtStartDate.SelectedDate.Value.Date);
                string srEndDate = String.Format("{0:s}", dtEndDate.SelectedDate.Value.Date);

                FormUrlEncodedContent formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>());
                HttpResponseMessage response = _client.PostAsync($"/api/Booking?carId={_car.CarId}&customerName={txtCustomerName.Text}&startDate={@srStartDate}&endDate={@srEndDate}", formContent).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception(responseString);

                txtResult.Visibility = Visibility.Visible;
                txtResult.Text = $"Succesfully book car.";
                txtResult.Background = Brushes.LightGreen;

                RefreshBookingList();
            }
            catch (Exception ex)
            {
                txtResult.Visibility = Visibility.Visible;
                txtResult.Text = $"Please fix input data. {ex.Message}";
                txtResult.Background = Brushes.IndianRed;

            }

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void DeleteBooking_Click(object sender, RoutedEventArgs e)
        {
            Booking objBook = (Booking)((Button)sender).Tag;
            if (objBook != null && MessageBox.Show($"Do you want delete {objBook.ToString()}?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                HttpResponseMessage resp = _client.DeleteAsync($"api/Booking?bookingId={objBook.BookingId}").Result;
                resp.EnsureSuccessStatusCode();

                MessageBox.Show($"Sucessfully deleted booking: {objBook.ToString()}.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshBookingList();
            }

        }
        #endregion

        #region "Interal Methods"
        private void RefreshBookingList()
        {
            HttpResponseMessage resp = _client.GetAsync($"api/Booking?rowIndex=0&rowForPage=1000&carId={_car.CarId}").Result;
            resp.EnsureSuccessStatusCode();
            List<Booking> lstBooking = resp.Content.ReadAsAsync<List<Booking>>().Result;
            dgBooking.ItemsSource = lstBooking;
        }

        private Task<List<Booking>> RefreshBookingListAsync()
        {
            HttpResponseMessage resp = _client.GetAsync($"api/Booking?rowIndex=0&rowForPage=1000&carId={_car.CarId}").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<Booking>>();
        }
        #endregion

    }

}
