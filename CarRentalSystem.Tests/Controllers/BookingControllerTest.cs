using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarRentalSystem;
using CarRentalSystem.Controllers;
using CarRentalSystem.Models;

namespace CarRentalSystem.Tests.Controllers
{
    [TestClass]
    public class BookingControllerTest
    {
        private static class DummyBooking
        {
            public const int CarId = 11;
            public const string CustomerName = "Test Customer";
        }

        private int paging_Index = 0;
        private int paging_MaxForPage = 1000;

        [TestMethod]
        public void Get()
        {
            BookingController controller = new BookingController();
            IEnumerable<Booking> result = controller.Get(paging_Index, paging_MaxForPage,1);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() >= 0);
        }
        [TestMethod]
        public void Post()
        {
            BookingController controller = new BookingController();
            try
            {
                controller.Post(498, "Customer 18", DateTime.Parse("1/22/2018 12:00:00 AM"), DateTime.Parse("1/31/2018 12:00:00 AM"));
                controller.Post(DummyBooking.CarId, DummyBooking.CustomerName, DateTime.Now, DateTime.Now.AddDays(5));
                IEnumerable<Booking> result = controller.Get(paging_Index, paging_MaxForPage, DummyBooking.CarId);
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Count() > 0);
            }
            catch (Exception)
            {
                controller.Delete(DummyBooking.CustomerName);
            }
        }

    }

}
