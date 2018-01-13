using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Windows.Markup;
using System.Globalization;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CarRentalSystemWPFClient
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        #region field
        private static HttpClient _client;
        public decimal PriceTo { get; set; }
        public decimal PriceFrom { get; set; }
        #endregion

        #region Costructor
        public MainWindow()
        {
            LoadInitialPageAsync();
            InitializeComponent();
            DataContext = this;
            FrameworkElement.LanguageProperty.OverrideMetadata(
                                        typeof(FrameworkElement),
                                        new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }
        #endregion

        #region WebAPI Methods
        private void HttpClientInit()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://gtrdbcarrentalsystem.azurewebsites.net");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void LoadInitialPageAsync()
        {
            if (_client == null)
                HttpClientInit();

            _client.GetAsync("api/Car?rowIndex=0&rowForPage=1000")
                .ContinueWith(x => UpdateUICars(x), TaskScheduler.FromCurrentSynchronizationContext());
        }

       private void UpdateUICars(Task<HttpResponseMessage> task)
        {
            HttpResponseMessage resp = task.Result;
            resp.EnsureSuccessStatusCode();
            List<Car> lstCars = resp.Content.ReadAsAsync<List<Car>>().Result;
            dgCars.ItemsSource = lstCars;
            this.PriceTo = lstCars.Select(x => x.PriceByDay).OrderByDescending(x => x).FirstOrDefault();
            this.PriceFrom = lstCars.Select(x => x.PriceByDay).OrderBy(x => x).FirstOrDefault();
            OnPropertyChanged(nameof(PriceFrom));
            OnPropertyChanged(nameof(PriceTo));
        }

       private void LoadCarsByPrices(string priceFrom, string priceTo)
        {
            HttpResponseMessage resp = _client.GetAsync($"/api/Car?rowIndex=0&rowForPage=1000&priceFrom={priceFrom}&priceTo={priceTo}").Result;
            resp.EnsureSuccessStatusCode();
            var lstCars = resp.Content.ReadAsAsync<List<Car>>().Result;
            dgCars.ItemsSource = lstCars;
        }

        #endregion

        #region Graphic Events
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadInitialPageAsync();
        }
        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            LoadCarsByPrices(PriceFrom.ToString(), PriceTo.ToString());
        }
        private void BookCar_Click(object sender, RoutedEventArgs e)
        {
            Car objCar = (Car)((Button)sender).Tag;
            if (objCar != null)
            {
                BookingPage pBooking = new BookingPage(objCar, _client);
                pBooking.ShowDialog();
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Car objCar = (Car)((Button)sender).Tag;
            if (objCar != null && MessageBox.Show("Are you sure that you want delete the car?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                HttpResponseMessage resp = _client.DeleteAsync($"api/Car?CarId={objCar.CarId}&asObsolete=False").Result;
                resp.EnsureSuccessStatusCode();
            }
            LoadInitialPageAsync();
            MessageBox.Show($"Sucessfully deleted Car: {objCar.ToString()}.","Information",MessageBoxButton.OK,MessageBoxImage.Information);
        }
        private void MakeObsolete_Click(object sender, RoutedEventArgs e)
        {
            Car objCar = (Car)((Button)sender).Tag;
            if (objCar != null)
            {
                HttpResponseMessage resp = _client.DeleteAsync($"api/Car?CarId={objCar.CarId}").Result;
                resp.EnsureSuccessStatusCode();
            }

           MessageBox.Show($"Car {objCar.ToString()} has set obsolete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void Duplicate_Click(object sender, RoutedEventArgs e)
        {
            Car objCar = (Car)((Button)sender).Tag;
            if (objCar != null)
            {
                HttpResponseMessage resp = _client.PostAsync($"api/Car?carName={objCar.Name}&brand={objCar.Brand}&pricePerDay={objCar.PriceByDay}", new FormUrlEncodedContent(new List<KeyValuePair<string, string>>())).Result;
                resp.EnsureSuccessStatusCode();

                LoadInitialPageAsync();
                MessageBox.Show($"Car {objCar.ToString()} has been duplicated.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

    }
}
