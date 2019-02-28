using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantReviewsAPI;
using RestaurantReviewsAPI.Controllers;
using RestaurantReviewsAPI.Models;
using RestaurantReviewsAPI.Models.DTOs;
using RestaurantReviewsAPI.Repositories;

namespace RestaurantReviewsAPI.Tests.Controllers
{
    [TestClass]
    public class LocationsControllerTest
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();
        static Mock<IRestaurantRepository> _restaurantRepo = new Mock<IRestaurantRepository>();
        static Mock<ILocationRepository> _locationRepo = new Mock<ILocationRepository>();
        static IList<Location> dbLocations;
        static IList<Restaurant> dbRestaurants;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _locationRepo.Setup(lr => lr.GetLocations()).Returns(dbLocations);
            _restaurantRepo.Setup(rr => rr.GetRestaurantsByLocation(It.IsAny<int>())).Returns((int id) =>
            {
                IList<Restaurant> restaurants = new List<Restaurant>();
                foreach (Restaurant rest in dbRestaurants)
                {
                    if (rest.Location.Id == id || rest.LocationId == id)
                        restaurants.Add(rest);
                }
                return restaurants;
            });
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // Data
            Location pittsburgh = new Location() { Id = 1, City = "Pittsburgh", State = "PA" };
            Location cranberry = new Location() { Id = 2, City = "Cranberry", State = "PA" };
            Location wexford = new Location() { Id = 3, City = "Wexford", State = "PA" };
            dbLocations = new List<Location> { pittsburgh, cranberry, wexford };

            Restaurant butcherAndRye = new Restaurant
            {
                Id = 1,
                Name = "Butcher and The Rye",
                Location = pittsburgh
            };
            Restaurant sienna = new Restaurant
            {
                Id = 2,
                Name = "Sienna Mercado",
                Location = pittsburgh
            };
            Restaurant carlton = new Restaurant
            {
                Id = 3,
                Name = "The Carlton Restaurant",
                Location = pittsburgh
            };
            Restaurant ichiban = new Restaurant
            {
                Id = 4,
                Name = "Ichiban Steak House & Sushi Bar",
                Location = cranberry
            };
            Restaurant brgr = new Restaurant
            {
                Id = 5,
                Name = "BRGR",
                Location = cranberry
            };
            Restaurant walnut = new Restaurant
            {
                Id = 6,
                Name = "Walnut Grill",
                Location = wexford
            };

            dbRestaurants = new List<Restaurant> { butcherAndRye, sienna, carlton, ichiban, brgr, walnut };
        }

        [TestMethod]
        public void GetLocations()
        {
            _locationRepo.Setup(lr => lr.GetLocations()).Returns(dbLocations);
            LocationsController controller = new LocationsController(_locationRepo.Object, _restaurantRepo.Object);

            // Act
            IHttpActionResult result = controller.GetLocations();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IList<LocationDTO>>));
            var contentResult = result as OkNegotiatedContentResult<IList<LocationDTO>>;
            Assert.IsNotNull(contentResult);
            IList<LocationDTO> locs = contentResult.Content;
            Assert.IsNotNull(locs);
            Assert.AreEqual(dbLocations.Count(), locs.Count());
        }

        [TestMethod]
        public void GetLocation_valid()
        {
            // Arrange
            LocationsController controller = new LocationsController(_locationRepo.Object, _restaurantRepo.Object);
            Location location = dbLocations.Last();
            _locationRepo.Setup(lr => lr.GetLocation(It.IsAny<int>())).Returns((int id) => dbLocations.SingleOrDefault(l => l.Id == id));

            // Act
            IHttpActionResult result = controller.GetLocation(location.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<LocationDTO>));
            var contentResult = result as OkNegotiatedContentResult<LocationDTO>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(location.Id, contentResult.Content.Id);
            Assert.AreEqual(location.City, contentResult.Content.City);
            Assert.AreEqual(location.State, contentResult.Content.State);
        }

        [TestMethod]
        public void GetLocation_invalid()
        {
            // Arrange
            LocationsController controller = new LocationsController(_locationRepo.Object, _restaurantRepo.Object);

            // Act
            IHttpActionResult result = controller.GetLocation(-1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetRestaurantsByLocation_valid()
        {
            // Arrange
            LocationsController controller = new LocationsController(_locationRepo.Object, _restaurantRepo.Object);
            Location location = dbLocations.Last();
            IList<Restaurant> restsAtLoc = dbRestaurants.Where(r => r.Location == location).ToList();
            // Act
            IHttpActionResult result = controller.Restaurants(location.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IList<RestaurantDTO>>));
            var contentResult = result as OkNegotiatedContentResult<IList<RestaurantDTO>>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(restsAtLoc.Count(), contentResult.Content.Count());
            foreach (RestaurantDTO restaurant in contentResult.Content)
            {
                Assert.AreEqual(location.Id, restaurant.Location.Id);
            }
        }

        [TestMethod]
        public void GetRestaurantsByLocation_invalid()
        {
            // Arrange
            LocationsController controller = new LocationsController(_locationRepo.Object, _restaurantRepo.Object);

            // Act
            IHttpActionResult result = controller.Restaurants(-1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IList<RestaurantDTO>>));
            var contentResult = result as OkNegotiatedContentResult<IList<RestaurantDTO>>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(0, contentResult.Content.Count());
        }
    }
}
