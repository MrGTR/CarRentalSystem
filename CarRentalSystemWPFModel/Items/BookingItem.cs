using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Base.WPFModel;

namespace Items.WPFModel
{
    public class BookingItem : PropertyBase
    {
        #region Costructor
        public BookingItem()
        {
            this.id = 0;
            this.startDate = DateTime.Now;
            this.endDate = DateTime.Now;
        }
        public BookingItem(CarItem objCar)
        {
            this.id = 0;
            this.startDate = DateTime.Now;
            this.endDate = DateTime.Now;
            this.Car = objCar;
        }
        #endregion

        private int id;
        public int Id
        {
            get { return this.id; }
            set
            {
                this.OnPropertyChanging(nameof(Id));
                this.id = value;
                this.OnPropertyChanged(nameof(Id));
            }
        }

        private string customerName;
        public string CustomerName
        {
            get { return this.customerName; }
            set
            {
                this.OnPropertyChanging(nameof(CustomerName));
                this.customerName = value;
                this.OnPropertyChanged(nameof(CustomerName));
                this.OnPropertyChanged(nameof(IsValidData));
            }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get { return this.startDate; }
            set
            {
                this.OnPropertyChanging(nameof(StartDate));
                this.startDate = value;
                this.OnPropertyChanged(nameof(StartDate));
                this.OnPropertyChanged(nameof(IsValidData));
            }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get { return this.endDate; }
            set
            {
                this.OnPropertyChanging(nameof(EndDate));
                this.endDate = value;
                this.OnPropertyChanged(nameof(EndDate));
                this.OnPropertyChanged(nameof(IsValidData));
            }
        }

        private CarItem car;
        public CarItem Car
        {
            get
            {
                return this.car;
            }
            set
            {
                this.OnPropertyChanging(nameof(Car));
                this.car = value;
                this.OnPropertyChanged(nameof(Car));
            }
        }

        public double Price
        {
            get
            {
                return ((this.endDate - this.startDate).TotalDays + 1) * (double)this.car.PriceByDay;
            }
        }

        public bool IsValidData
        {
            get
            {
                if (id == 0)
                    return true;

                if (this.StartDate == null || this.EndDate == null || String.IsNullOrEmpty(this.CustomerName))
                    return false;

                if (this.StartDate < DateTime.Today || this.EndDate < this.StartDate)
                    return false;

                return true;
            }
        }

        public override string ToString()
        {
            if (id == 0)
                return $"New Reservation";
            else
                return $"Reservation Number = {id.ToString()}";
        }

    }
}
