using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Configuration;
using WPFBiz;
using WPFBiz.DataFetcher;
using WPFBiz.DataLayer;
using System.Windows.Input;
using System.Threading.Tasks;
using Base.WPFModel;
using Items.WPFModel;

namespace WPFModel
{
    public class CarRentalViewModel : ViewModelBase
    {
        #region Costructor
        public CarRentalViewModel()
        {
         this._cars = LoadCars();
        }
        #endregion

        #region Common
        public string url
        {
            get
            {
                return "http://gtrdbcarrentalsystem.azurewebsites.net";
            }
        }

        private DataManager _httpClient;
        public DataManager DbFactory
        {
            get
            {
                if (this._httpClient == null)
                {
                    string url = this.url;
                    this._httpClient = new DataManager(url);
                }
                return this._httpClient;
            }
        }

        #endregion

        #region Cars
        private ObservableCollection<CarItem> _cars;
        public ObservableCollection<CarItem> Cars
        {
            get
            {
                if (this._cars == null)
                    this._cars = this.LoadCars();
                return this._cars;

            }
        }
        private ObservableCollection<CarItem> LoadCars()
        {
            ObservableCollection<CarItem> items = new ObservableCollection<CarItem>();

            List<Car> cars = CarFetcher.GetCars(this.DbFactory);

            cars.ForEach(x => {
                items.Add(new CarItem { Id = x.CarId, Brand = x.Brand, Name = x.Name, PriceByDay = x.PriceByDay });
            });

            OnPropertyChanged(nameof(MaxPriceCars));
            OnPropertyChanged(nameof(MinPriceCars));

            return items;

        }
        private ObservableCollection<CarItem> LoadCarsByPrices()
        {
            ObservableCollection<CarItem> items = new ObservableCollection<CarItem>();

            List<Car> cars = CarFetcher.GetCars(this.MinPriceCars,this.MaxPriceCars,this.DbFactory);

            cars.ForEach(x => {
                items.Add(new CarItem { Id = x.CarId, Brand = x.Brand, Name = x.Name, PriceByDay = x.PriceByDay });
            });

            OnPropertyChanged(nameof(MaxPriceCars));
            OnPropertyChanged(nameof(MinPriceCars));

            return items;

        }
        private decimal? _max;
        public decimal MaxPriceCars
        {
            get  { return _max ?? this._cars.Max(x => x.PriceByDay);}
            set { _max = value; }
        }
        private decimal? _min;
        public decimal MinPriceCars
        {
            get { return _min ?? this._cars.Min(x=> x.PriceByDay); }
            set { _min = value;}
        }
        private CommandBase getCars;
        public CommandBase GetCars
        {
            get
            {
                if (getCars == null)
                {
                    getCars = new CommandBase(param => {

                        this._cars = this.LoadCars();
                        OnPropertyChanged(nameof(Cars));

                    }, param => true);
                }
                return getCars;
            }
        }
        private CarItem selectedCar;
        public CarItem SelectedCar
        {
            get { return this.selectedCar; }
            set
            {
                this.OnPropertyChanging(nameof(EnableBookinglist)); 
                this.OnPropertyChanging(nameof(SelectedCar));
                this.selectedCar = value;
                GetBookingByCarId();
                CreateBooking(this.selectedCar);
                this.OnPropertyChanged(nameof(SelectedCar));
                this.OnPropertyChanged(nameof(EnableBookinglist));
            }
        }

        public bool EnableBookinglist
        {
            get
            {
                return this.selectedCar != null;
            }
        }

        #endregion

        #region Booking

        private ObservableCollection<BookingItem> booking = new ObservableCollection<BookingItem>();
        public ObservableCollection<BookingItem> Bookings
        {
            get { return this.booking; }
        }
        private void GetBookingByCarId()
        {
            OnPropertyChanging(nameof(Bookings));

            this.booking.Clear();
            List<Booking> bookings = BookingFetcher.GetBookingByCarId(this.selectedCar.Id, this.DbFactory);
            bookings.ForEach(x => {
                this.booking.Add(new BookingItem { Id = x.BookingId, CustomerName = x.CustomerName, Car = this.selectedCar, StartDate = x.StartDate, EndDate = x.EndDate });
            });

            OnPropertyChanged(nameof(Bookings));
        }
        private void DeleteBooking()
        {
            BookingFetcher.DeleteBooking(this.currentBooking.Id, this.DbFactory);
        }
        private object CreateBooking(CarItem objCar)
        {
            this.CurrentBooking = new BookingItem(objCar);
            return this.CurrentBooking;
        }
        private BookingItem currentBooking;
        public BookingItem CurrentBooking
        {
            get { return this.currentBooking; }
            set
            {
                this.OnPropertyChanging(nameof(CanDeleteBooking));
                this.OnPropertyChanging(nameof(CanSaveBooking));
                this.OnPropertyChanging(nameof(CurrentBooking));
                this.currentBooking = value;
                this.OnPropertyChanged(nameof(CurrentBooking));
                this.OnPropertyChanged(nameof(CanDeleteBooking));
                this.OnPropertyChanged(nameof(CanSaveBooking));
            }
        }

        private CommandBase saveBooking;
        public CommandBase SaveBooking
        {
            get
            {
                if (saveBooking == null)
                {
                    saveBooking = new CommandBase(param => this.SaveCurrentBooking(), param => this.CanSaveBooking);
                }
                return saveBooking;
            }
        }

        public object SaveCurrentBooking()
        {
            if (this.CanSaveBooking)
            {
                if (this.IsNewBooking)
                    BookingFetcher.SaveBooking(new Booking { CustomerName = this.CurrentBooking.CustomerName, CarId = this.CurrentBooking.Car.Id, StartDate = this.CurrentBooking.StartDate, EndDate = this.CurrentBooking.EndDate }, this.DbFactory);
                else
                    BookingFetcher.UpdateBooking(new Booking { CustomerName = this.CurrentBooking.CustomerName, CarId = this.CurrentBooking.Car.Id, StartDate = this.CurrentBooking.StartDate, EndDate = this.CurrentBooking.EndDate }, this.DbFactory);

                GetBookingByCarId();
            }
            return this.CurrentBooking;
        }

        private CommandBase filterCars;
        public CommandBase FilterCars
        {
            get
            {
                if (filterCars == null)
                {
                    filterCars = new CommandBase(param => {

                        this._cars = this.LoadCarsByPrices();
                        OnPropertyChanged(nameof(Cars));

                    }, param => true);
                }
                return filterCars;
            }
        }

        private CommandBase deleteReservation;
        public CommandBase DeleteReservation
        {
            get
            {
                if (deleteReservation == null)
                    deleteReservation = new CommandBase(param => {

                        this.DeleteBooking();
                        GetBookingByCarId();
                        OnPropertyChanging(nameof(CurrentBooking));
                        this.currentBooking = new BookingItem(this.selectedCar);
                        OnPropertyChanged(nameof(CurrentBooking));

                    }
                    , param => true);

                return deleteReservation;
            }
        }

        public bool IsNewBooking
        {
            get
            {
                if (this.currentBooking == null || this.currentBooking.Id == 0)
                    return true;

                return false;
            }
        }
        public bool canSave = false;
        public bool CanSaveBooking
        {
            get
            {
                return this.currentBooking?.IsValidData ?? false;
            }
        }
        public bool CanDeleteBooking
        {
            get
            {
                return this.currentBooking != null;
            }
        }

        #endregion
    }
}
