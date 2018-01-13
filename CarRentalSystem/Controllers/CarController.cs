using CarRentalSystem.Common;
using CarRentalSystem.Models;
using Dapper;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace CarRentalSystem.Controllers
{

    public class CarController : CustomApiController
    {
        #region Costructor
        public CarController() : base(MethodBase.GetCurrentMethod().DeclaringType)
        {
        }
        #endregion

        #region GET
        /// <summary>
        /// Get cars 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Car> Get(int rowIndex, int rowForPage)
        {
            _log.Debug($"{MethodBase.GetCurrentMethod().Name} - {nameof(rowIndex)}={rowIndex.ToString()} {nameof(rowForPage)}={rowForPage.ToString()}");
            IEnumerable<Car> cars =  this.GetMany<Car>("spGet_Cars", new { startRowIndex = rowIndex, maximumRows = rowForPage });
            return cars;
        }

        /// <summary>
        /// Get Cars by Id
        /// </summary>
        /// <param name="carId"></param>
        /// <returns></returns>
        public Car Get(int carId)
        {
            _log.Debug($"{MethodBase.GetCurrentMethod().Name} - {nameof(carId)}={carId.ToString()}");
            Car car = this.Get<Car>("spGet_CarById", new { CarId = carId });
            return car;
        }

        /// <summary>
        /// Get Cars by Price Range
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Car> Get(int rowIndex, int rowForPage,decimal priceFrom, decimal priceTo)
        {
            _log.Debug($"{MethodBase.GetCurrentMethod().Name} - {nameof(rowIndex)}={rowIndex.ToString()} {nameof(rowForPage)}={rowForPage.ToString()} {nameof(priceFrom)}={priceFrom.ToString()} {nameof(priceTo)}={priceTo.ToString()}");
            IEnumerable<Car> cars = this.GetMany<Car>("spGet_Cars", new { startRowIndex = rowIndex, maximumRows = rowForPage, PriceFrom = priceFrom, PriceTo = priceTo });
            return cars;
        }

        #endregion

        #region POST
        /// <summary>
        /// Create a new car
        /// </summary>
        /// <param name="carName"></param>
        /// <param name="brand"></param>
        /// <param name="pricePerDay"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(string carName, string brand, decimal pricePerDay)
        {
            _log.Debug($"{MethodBase.GetCurrentMethod().Name} - {nameof(carName)}={carName} {nameof(brand)}={brand} {nameof(pricePerDay)}={pricePerDay.ToString()}");

            Action aPost = (Action)(() =>
            {
                using (IDbConnection db = new SqlConnection(Constants.AzureDatabase.ConnectionString))
                {
                    if (pricePerDay <= 0 || String.IsNullOrEmpty(carName) || String.IsNullOrEmpty(brand))
                        throw new Exception($"Input parameter are not valid.");

                    db.Execute("spAdd_Car", new { Name = carName, Brand = brand, PriceByDay = pricePerDay }, commandType: CommandType.StoredProcedure);

                    _log.Info($"Car correctly added. Name = {carName} Brand = {brand} PriceByDay = {pricePerDay.ToString()}");
                }
            });

           return this.Execute(aPost);

        }
        #endregion

        #region PUT
        /// <summary>
        /// Update an Existing Car
        /// </summary>
        /// <param name="carId"></param>
        /// <param name="carName"></param>
        /// <param name="brand"></param>
        /// <param name="pricePerDay"></param>
        /// <returns></returns>
        public HttpResponseMessage Put(int carId,string carName, string brand, decimal pricePerDay)
        {
            _log.Debug($"{MethodBase.GetCurrentMethod().Name} - {nameof(carId)}={carId} {nameof(carName)}={carName} {nameof(brand)}={brand} {nameof(pricePerDay)}={pricePerDay.ToString()}");

            Action aPut = (Action)(() =>
            {
                using (IDbConnection db = new SqlConnection(Constants.AzureDatabase.ConnectionString))
                {
                    if (pricePerDay <= 0 || String.IsNullOrEmpty(carName) || String.IsNullOrEmpty(brand))
                        throw new Exception($"Input parameter are not valid.");

                    Car car = db.Query<Car>("spGet_CarById", new { CarId = carId }, commandType: CommandType.StoredProcedure).FirstOrDefault();

                    if (car == null) throw new Exception($"CarId = {carId.ToString()} not exist.");
                    if (car.IsObsolete) throw new Exception($"CarId = {carId.ToString()} is Obsolete.");

                    db.Execute("spUpd_Car", new { CarId = carId, Name = carName, Brand = brand, PriceByDay = pricePerDay }, commandType: CommandType.StoredProcedure);

                    _log.Info($"Car correctly updated. {car.ToString()}");
                }
            });

            return this.Execute(aPut);
        }

        #endregion

        #region DELETE
        /// <summary>
        /// Set a car obsolete
        /// </summary>
        /// <param name="carId"></param>
        public HttpResponseMessage Delete(int carId)
        {
            _log.Debug($"{MethodBase.GetCurrentMethod().Name} - {nameof(carId)}={carId} asObsolete=True.");

            Action aDel = (Action)(() =>
            {
                using (IDbConnection db = new SqlConnection(Constants.AzureDatabase.ConnectionString))
                {
                    IEnumerable<Car> cars = db.Query<Car>("spGet_CarById", new { CarId = carId }, commandType: CommandType.StoredProcedure);
                    if (!cars.HasElements())
                        throw new Exception($"CarId = {carId.ToString()} not exist.");

                    db.Execute("spDel_Car", new { CarId = carId, AsObsolete = 1 }, commandType: CommandType.StoredProcedure);
                    _log.Info($"A car has been deleted. Details: {cars.First().ToString()}");
                }
            });

            return this.Execute(aDel);
        }

        /// <summary>
        /// if asObsolete = false delete car and its bookings
        /// </summary>
        /// <param name="carId"></param>
        /// <param name="asObsolete"></param>
        public HttpResponseMessage Delete(int carId, bool asObsolete)
        {
            _log.Debug($"{MethodBase.GetCurrentMethod().Name} - {nameof(carId)}={carId} {nameof(asObsolete)}={asObsolete}");

            Action aDel = (Action)(() =>
            {
                using (IDbConnection db = new SqlConnection(Constants.AzureDatabase.ConnectionString))
            {
                IEnumerable<Car> car = db.Query<Car>("spGet_CarById", new { CarId = carId }, commandType: CommandType.StoredProcedure);

                if (!car.HasElements())
                    throw new Exception($"CarId = {carId.ToString()} not exist.");

                db.Execute("spDel_Car", new { CarId = carId, AsObsolete = asObsolete }, commandType: CommandType.StoredProcedure);

                _log.Info($"A car has been deleted. Details: {car.First().ToString()}");
                }
            });

        return this.Execute(aDel);
        }

        #endregion
    }
}
