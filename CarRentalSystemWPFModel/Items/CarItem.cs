using Base.WPFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Items.WPFModel
{
    public class CarItem : PropertyBase 
    {
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

        private string brand;
        public string Brand
        {
            get { return this.brand; }
            set
            {
                this.OnPropertyChanging(nameof(Brand));
                this.brand = value;
                this.OnPropertyChanged(nameof(Brand));
            }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                this.OnPropertyChanging(nameof(Name));
                this.name = value;
                this.OnPropertyChanged(nameof(Name));
            }
        }

        private decimal priceByDay;
        public decimal PriceByDay
        {
            get { return this.priceByDay; }
            set
            {
                this.OnPropertyChanging(nameof(PriceByDay));
                this.priceByDay = value;
                this.OnPropertyChanged(nameof(PriceByDay));
            }
        }

        public override string ToString()
        {
            return $"Plate Number: {this.id} Type: {this.name} ({this.brand})";
        }
    }
}
