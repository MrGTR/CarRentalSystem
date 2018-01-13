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
using CarRentalSystem.Common;

namespace CarRentalSystem.Tests.Controllers
{
    [TestClass]
    public class CarControllerTest
    {
        private static class DummyCar1
        {
            public const int CarId = 1;
            public const string Name = "A Class";
            public const string Brand = "Mercedes-Benz";
            public const decimal PriceByDay = 100;
        }
        private static class DummyCar2
        {
            public const int CarId = 2;
            public const string Name = "Punto";
            public const string Brand = "FIAT";
            public const decimal PriceByDay = 50;
        }

        private int paging_Index = 0;
        private int paging_MaxForPage = 1000;

        [TestMethod]
        public void Get()
        {
            // Arrange
            CarController controller = new CarController();

            // Act
            IEnumerable<Car> result = controller.Get(paging_Index, paging_MaxForPage);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
        }

        [TestMethod]
        public void GetById()
        {
            // Arrange
            CarController controller = new CarController();
            // Act
            Car result = controller.Get(4);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetByPrices()
        {
            // Arrange
            CarController controller = new CarController();

            IEnumerable<Car> allCars = controller.Get(paging_Index, paging_MaxForPage);
            Assert.IsTrue(allCars.HasElements());

            int resTest1 = allCars.Where(x => x.PriceByDay <= 50).Count();
            IEnumerable<Car> carsTest1 = controller.Get(paging_Index, paging_MaxForPage, 0, 50);
            Assert.IsTrue(carsTest1.HasElements());
            Assert.IsTrue(carsTest1.Count() == resTest1);

            int resTest2 = allCars.Where(x => x.PriceByDay >= 50 && x.PriceByDay <= 1000).Count();
            IEnumerable<Car> carsTest2 = controller.Get(paging_Index, paging_MaxForPage, 50, 1000);
            Assert.IsTrue(carsTest1.HasElements());
            Assert.IsTrue(carsTest1.Count() == resTest1);
        }

        [TestMethod]
        public void GetWithPaging()
        {
            // Arrange
            CarController controller = new CarController();

            // Act
            List<Car> allCars = controller.Get(paging_Index, paging_MaxForPage).ToList();
            Assert.IsTrue(allCars.HasElements());

            List<Car> enumCars1 = controller.Get(0, 3).ToList();
            Assert.IsTrue(allCars[0].CarId == enumCars1[0].CarId);
            Assert.IsTrue(allCars[1].CarId == enumCars1[1].CarId);
            Assert.IsTrue(allCars[2].CarId == enumCars1[2].CarId);


            List<Car> enumCars2 = controller.Get(3, 3).ToList();
            Assert.IsTrue(allCars[3].CarId == enumCars2[0].CarId);
            Assert.IsTrue(allCars[4].CarId == enumCars2[1].CarId);
            Assert.IsTrue(allCars[5].CarId == enumCars2[2].CarId);

            List<Car> enumCars3 = controller.Get(6, 3).ToList();
            Assert.IsTrue(allCars[6].CarId == enumCars3[0].CarId);
            Assert.IsTrue(allCars[7].CarId == enumCars3[1].CarId);
            Assert.IsTrue(allCars[8].CarId == enumCars3[2].CarId);

        }

        [TestMethod]
        public void Post()
        {
            // Arrange
            CarController controller = new CarController();
            int carId2Delte = DummyCar1.CarId;
            try
            {
                HttpResponseMessage retMsg =  controller.Post(DummyCar1.Name, DummyCar1.Brand, DummyCar1.PriceByDay);
                Assert.IsTrue(retMsg.StatusCode == System.Net.HttpStatusCode.OK);

                IEnumerable<Car> result = controller.Get(1, 1000);
                Assert.IsTrue(result.HasElements());

                Assert.AreEqual(result.Last().Name, DummyCar1.Name);
                Assert.AreEqual(result.Last().Brand, DummyCar1.Brand);
                Assert.AreEqual(result.Last().PriceByDay, DummyCar1.PriceByDay);

                carId2Delte = result.Last().CarId;
            }
            finally
            {
                controller.Delete(carId2Delte, false);
            }
        }

        [TestMethod]
        public void Put()
        {
            // Arrange
            CarController controller = new CarController();
            int carId2Delte = DummyCar1.CarId;
            try
            {
                HttpResponseMessage retMsg = controller.Post(DummyCar1.Name, DummyCar1.Brand, DummyCar1.PriceByDay);
                Assert.IsTrue(retMsg.StatusCode == System.Net.HttpStatusCode.OK);

                IEnumerable<Car> result = controller.Get(1, 1000);
                Assert.IsTrue(result.HasElements());

                Assert.AreEqual(result.Last().Name, DummyCar1.Name);
                Assert.AreEqual(result.Last().Brand, DummyCar1.Brand);
                Assert.AreEqual(result.Last().PriceByDay, DummyCar1.PriceByDay);

                carId2Delte = result.Last().CarId;

                controller.Put(carId2Delte, DummyCar2.Name, DummyCar2.Brand, DummyCar2.PriceByDay);

                result = controller.Get(1, 1000);

                Assert.AreEqual(result.Last().Name, DummyCar2.Name);
                Assert.AreEqual(result.Last().Brand, DummyCar2.Brand);
                Assert.AreEqual(result.Last().PriceByDay, DummyCar2.PriceByDay);

            }
            finally
            {
                controller.Delete(carId2Delte, false);
            }
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            CarController controller = new CarController();
            int carId2Delte = DummyCar1.CarId;
            try
            {
                HttpResponseMessage retMsg = controller.Post(DummyCar1.Name, DummyCar1.Brand, DummyCar1.PriceByDay);
                Assert.IsTrue(retMsg.StatusCode == System.Net.HttpStatusCode.OK);

                IEnumerable<Car> result = controller.Get(1, 1000);
                Assert.IsTrue(result.HasElements());

                carId2Delte = result.Last().CarId;
                controller.Delete(carId2Delte);

                Car car =  controller.Get(carId2Delte);
                Assert.IsNotNull(car);
                Assert.IsTrue(car.IsObsolete);

                controller.Delete(carId2Delte,false);

                car = controller.Get(carId2Delte);
                Assert.IsNull(car);
            }
            finally
            {
                controller.Post(DummyCar1.Name, DummyCar1.Brand, DummyCar1.PriceByDay);
            }
        }
    }
}
