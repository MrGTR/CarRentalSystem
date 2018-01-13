using CarRentalSystem.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CarRentalSystem.Common;
using log4net;
using System.Reflection;

namespace CarRentalSystem.Controllers
{
    public class BookingController : CustomApiController
    {
        #region Costructor
        public BookingController() : base(MethodBase.GetCurrentMethod().DeclaringType)
        {
        }
        #endregion

        /// <summary>
        /// get booking list by CarId
        /// </summary>
        /// <param name="carId"></param>
        /// <returns></returns>
        public IEnumerable<Booking> Get(int rowIndex, int rowForPage, int carId)
        {
            _log.Debug($"{nameof(Get)} - {nameof(rowIndex)}={rowIndex.ToString()} {nameof(rowForPage)}={rowForPage.ToString()} {nameof(carId)}={carId.ToString()}");
            IEnumerable<Booking> booking = this.GetMany<Booking>("spGet_BookingByCarId", new { startRowIndex = rowIndex, maximumRows = rowForPage, CarId = carId });
            return booking;
        }

        /// <summary>
        /// Book a car for a certain period of time
        /// </summary>
        /// <param name="carId"></param>
        /// <param name="customerName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public HttpResponseMessage Post(int carId,string customerName, DateTime startDate, DateTime endDate)
        {
            _log.Debug($"{nameof(Post)} - {nameof(carId)}={carId.ToString()} {nameof(customerName)}={customerName} {nameof(startDate)}={startDate.ToString()}  {nameof(endDate)}={endDate.ToString()}");

            Action aPost = (Action)(() =>
            {
                using (IDbConnection db = new SqlConnection(Constants.AzureDatabase.ConnectionString))
                {

                    if (startDate < DateTime.Today)
                        throw new Exception("Start date is not valid. Must be at least today.");

                    IEnumerable<Booking> booking = db.Query<Booking>("spGet_BookingByCarId", new { startRowIndex = 0, maximumRows = 1000, CarId = carId, StartDate = startDate, EndDate = endDate }, commandType: CommandType.StoredProcedure);

                    if (booking.HasElements())
                        throw new Exception("Car already booked for selected period.");

                    db.Execute("spAdd_Booking", new { CarId = carId, StartDate = startDate, EndDate = endDate, CustomerName = customerName }, commandType: CommandType.StoredProcedure);

                    _log.Info($"Car with Id = {carId}, corretly booked.");
                }
            });

            return this.Execute(aPost);
        }

        /// <summary>
        /// Delete booking
        /// </summary>
        /// <param name="bookingId"></param>
        public void Delete(int bookingId)
        {
            _log.Debug($"{nameof(Delete)} - {nameof(bookingId)}={bookingId.ToString()}");
            this.ExecuteStoredProcedure("spDel_Booking", new { BookingId = bookingId });
        }

        /// <summary>
        /// Delete all booking for input customer name
        /// </summary>
        /// <param name="customerName"></param>
        public void Delete(string customerName)
        {
            _log.Debug($"{nameof(Delete)} - {nameof(customerName)}={customerName.ToString()}");
            this.ExecuteStoredProcedure("spDel_BookingByCustomerName", new { CustomerName = customerName });
        }
    }
}