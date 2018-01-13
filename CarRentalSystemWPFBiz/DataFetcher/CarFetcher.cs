using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WPFBiz.DataLayer;

namespace WPFBiz.DataFetcher
{
    public static class CarFetcher
    {
        public static Car GetCar(int carId,DataManager manager)
        {
            string url = $"api/Car?carId={carId.ToString()}";
            Car objEntity = manager.Get<Car>(url);
            if (objEntity != null)
            {
                return objEntity;
            }
            return new Car();
        }
        public static List<Car> GetCars(DataManager manager)
        {
            string url = "api/Car?rowIndex=0&rowForPage=1000";
            List<Car> objEntity = manager.GetMany<Car>(url);
            if (objEntity != null)
            {
                return objEntity;
            }
            return new List<Car>();
        }
        public static List<Car> GetCars(decimal from, decimal to,DataManager manager)
        {
            string url = $"api/Car?rowIndex=0&rowForPage=1000&priceFrom={from.ToString()}&priceTo={to.ToString()}";
            List<Car> objEntity = manager.GetMany<Car>(url);
            if (objEntity != null)
            {
                return objEntity;
            }
            return new List<Car>();
        }
    }
}
