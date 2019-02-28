using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestaurantReviewsAPI.Controllers;
using RestaurantReviewsAPI.Models;
using RestaurantReviewsAPI.Repositories;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantReviewsAPI.Models.DTOs;
using Moq;
using System.Web.Http;
using System.Web.Http.Results;

namespace RestaurantReviewsAPI.Tests.Controllers
{
    [TestClass]
    public class RestaurantsControllerTest
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();
        static Mock<IRestaurantRepository> _restaurantRepo = new Mock<IRestaurantRepository>();
        static Mock<ILocationRepository> _locationRepo = new Mock<ILocationRepository>();
        static Mock<IReviewRepository> _reviewRepo = new Mock<IReviewRepository>();
        static IList<Restaurant> dbRestaurants;
        static IList<Location> dbLocations = new List<Location>();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Mocks
            _restaurantRepo.Setup(restR => restR.Get(It.IsAny<int>())).Returns((int id) => dbRestaurants.SingleOrDefault(r => r.Id == id));
            _restaurantRepo.Setup(restR => restR.GetRestaurant(It.IsAny<int>())).Returns((int id) => dbRestaurants.SingleOrDefault(r => r.Id == id));
            _restaurantRepo.Setup(restR => restR.GetRestaurantByNameAndLocationId(It.IsAny<string>(), It.IsAny<int>()))
                .Returns((string name, int locId) => dbRestaurants.SingleOrDefault(res => res.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && res.Location.Id == locId));
            _restaurantRepo.Setup(restR => restR.AddRestaurant(It.IsAny<Restaurant>())).Returns((Restaurant rest) =>
            {
                int maxId = dbRestaurants.Max(r => r.Id);
                rest.Id = ++maxId;
                dbRestaurants.Add(rest);
                var taskSource = new TaskCompletionSource<Restaurant>();
                taskSource.SetResult(rest);
                return taskSource.Task;
            });
            _restaurantRepo.Setup(restR => restR.UpdateRestaurant(It.IsAny<int>(), It.IsAny<Restaurant>())).Returns((int id, Restaurant rest) =>
            {
                if (rest.Location == null)
                {
                    rest.Location = dbLocations.SingleOrDefault(l => l.Id == rest.LocationId);
                }
                for (var i = 0; i < dbRestaurants.Count(); i++)
                {
                    if (dbRestaurants[i].Id == id)
                    {
                        dbRestaurants[i].Name = rest.Name;
                        dbRestaurants[i].Location = rest.Location;
                        dbRestaurants[i].LocationId = rest.LocationId;
                        return true;
                    }
                }
                return false;
            });
            _locationRepo.Setup(loc => loc.GetLocation(It.IsAny<int>())).Returns((int id) => dbLocations.SingleOrDefault(l => l.Id == id));
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
        public void GetRestaurants()
        {
            // Arrange
            _restaurantRepo.Setup(restR => restR.GetRestaurants()).Returns(dbRestaurants);
            // Act and Assert
            Test_GetRestaurants(dbRestaurants.Count());
        }

        [TestMethod]
        public void GetRestaurants_null()
        {
            // Arrange
            _restaurantRepo.Setup(restR => restR.GetRestaurants()).Returns((IList<Restaurant>)null);
            // Act and Assert
            Test_GetRestaurants(0);
        }

        [TestMethod]
        public void GetRestaurants_empty()
        {
            // Arrange
            _restaurantRepo.Setup(restR => restR.GetRestaurants()).Returns(new List<Restaurant>());
            // Act and Assert
            Test_GetRestaurants(0);
        }

        private void Test_GetRestaurants(int expectedCount)
        {
            RestaurantsController controller = new RestaurantsController(_locationRepo.Object, _restaurantRepo.Object, _reviewRepo.Object);

            // Act
            //int restCount = _db.Restaurants.Count();
            IHttpActionResult result = controller.GetRestaurants();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IList<RestaurantDTO>>));
            var contentResult = result as OkNegotiatedContentResult<IList<RestaurantDTO>>;
            Assert.IsNotNull(contentResult);
            IList<RestaurantDTO> rests = contentResult.Content;
            Assert.IsNotNull(rests);
            Assert.AreEqual(expectedCount, rests.Count());
        }

        [TestMethod]
        public void GetRestaurant_valid()
        {
            // Arrange
            RestaurantsController controller = new RestaurantsController(_locationRepo.Object, _restaurantRepo.Object, _reviewRepo.Object);
            Restaurant restaurant = dbRestaurants.Last();
            _restaurantRepo.Setup(restR => restR.GetRestaurant(It.IsAny<int>())).Returns((int id) => dbRestaurants.SingleOrDefault(r => r.Id == id));

            // Act
            IHttpActionResult result = controller.GetRestaurant(restaurant.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<RestaurantDTO>));
            var contentResult = result as OkNegotiatedContentResult<RestaurantDTO>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(restaurant.Id, contentResult.Content.Id);
            Assert.AreEqual(restaurant.Name, contentResult.Content.Name);
            Assert.IsNotNull(contentResult.Content.Location);
            Assert.AreEqual(restaurant.Location.Id, contentResult.Content.Location.Id);
            Assert.AreEqual(restaurant.Location.City, contentResult.Content.Location.City);
            Assert.AreEqual(restaurant.Location.State, contentResult.Content.Location.State);
        }

        [TestMethod]
        public void GetRestaurant_invalid()
        {
            // Arrange
            RestaurantsController controller = new RestaurantsController(_locationRepo.Object, _restaurantRepo.Object, _reviewRepo.Object);

            // Act
            IHttpActionResult result = controller.GetRestaurant(-1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PostRestaurant_BadModelState()
        {
            // Arrange
            RestaurantsController controller = new RestaurantsController(_locationRepo.Object, _restaurantRepo.Object, _reviewRepo.Object);
            controller.ModelState.AddModelError("ERROR!!!", "eee gads");

            // Act
            IHttpActionResult result = controller.PostRestaurant(new Restaurant { }).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void PostRestaurant_EmptyRestaurant()
        {
            PostRestaurantWithBadData(new Restaurant { });
        }

        [TestMethod]
        public void PostRestaurant_NullRestaurant()
        {
            PostRestaurantWithBadData(null);
        }

        [TestMethod]
        public void PostRestaurant_InvalidRestaurantData_EmptyName()
        {
            // Arrange
            Restaurant restaurant = dbRestaurants.First();
            restaurant.Name = "";

            // Act && Assert
            PostRestaurantWithBadData(restaurant);
        }

        [TestMethod]
        public void PostRestaurant_InvalidRestaurantData_MissingLocation()
        {
            Restaurant restaurant = dbRestaurants.First();
            restaurant.Location = null;
            PostRestaurantWithBadData(restaurant);
        }

        [TestMethod]
        public void PostRestaurant_InvalidRestaurantData_AlreadyExists()
        {
            Restaurant restaurant = dbRestaurants.First();
            PostRestaurantWithBadData(restaurant);
        }

        [TestMethod]
        public void PostRestaurant_NewRestaurant()
        {
            // Arrange
            RestaurantsController controller = new RestaurantsController(_locationRepo.Object, _restaurantRepo.Object, _reviewRepo.Object);
            Restaurant restaurant = new Restaurant {
                Name = "Test Restaurant",
                Location = dbLocations.First()
            };
            int maxId = dbRestaurants.Max(r => r.Id);
            int restaurantsCount = dbRestaurants.Count();

            // Act
            IHttpActionResult result = controller.PostRestaurant(restaurant).Result;
            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<Restaurant>));
            var contentResult = result as CreatedAtRouteNegotiatedContentResult<Restaurant>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.Id > 0);
            Assert.AreEqual(restaurant.Name, contentResult.Content.Name);
            Assert.IsNotNull(contentResult.Content.Location);
            Assert.AreEqual(restaurant.Location.Id, contentResult.Content.Location.Id);
            Assert.AreEqual(restaurant.Location.City, contentResult.Content.Location.City);
            Assert.AreEqual(restaurant.Location.State, contentResult.Content.Location.State);
            Assert.AreEqual(restaurantsCount + 1, dbRestaurants.Count());
            Assert.AreEqual(contentResult.Content.Id, maxId + 1);
        }

        [TestMethod]
        public void PostRestaurant_NewRestaurantLocation_ViaPostMethod()
        {
            PostRestaurant_NewRestaurantLocation(true);
        }

        [TestMethod]
        public void PostRestaurant_NewRestaurantLocation_ViaAddRestaurantLocation()
        {
            PostRestaurant_NewRestaurantLocation(false);
        }

        private void PostRestaurant_NewRestaurantLocation(bool post)
        {
            // Arrange
            RestaurantsController controller = new RestaurantsController(_locationRepo.Object, _restaurantRepo.Object, _reviewRepo.Object);
            Restaurant firstRest = dbRestaurants.First();
            Restaurant restaurant = new Restaurant
            {
                Location = firstRest.Location,
                Name = firstRest.Name
            };
            if (post)
                restaurant.Id = -1;
            else
                restaurant.Id = firstRest.Id;

            Location currentLocation = restaurant.Location;
            foreach (Location loc in dbLocations)
            {
                if (loc != currentLocation)
                {
                    restaurant.Location = loc;
                    break;
                }
            }
            int maxId = dbRestaurants.Max(r => r.Id);
            int restaurantsCount = dbRestaurants.Count();

            // Act
            IHttpActionResult result = post ? controller.PostRestaurant(restaurant).Result : controller.AddRestaurantLocation(restaurant).Result;
            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<Restaurant>));
            var contentResult = result as CreatedAtRouteNegotiatedContentResult<Restaurant>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.Id > 0);
            Assert.AreEqual(restaurant.Name, contentResult.Content.Name);
            Assert.IsNotNull(contentResult.Content.Location);
            Assert.AreEqual(restaurant.Location.Id, contentResult.Content.Location.Id);
            Assert.AreEqual(restaurant.Location.City, contentResult.Content.Location.City);
            Assert.AreEqual(restaurant.Location.State, contentResult.Content.Location.State);
            Assert.AreEqual(restaurantsCount + 1, dbRestaurants.Count());
            Assert.AreEqual(contentResult.Content.Id, maxId + 1);
        }

        [TestMethod]
        public void PostRestaurant_ChangeRestaurantLocation()
        {
            // Arrange
            RestaurantsController controller = new RestaurantsController(_locationRepo.Object, _restaurantRepo.Object, _reviewRepo.Object);
            Restaurant firstRest = dbRestaurants.First();
            Restaurant restaurant = new Restaurant
            {
                Id = firstRest.Id,
                Location = firstRest.Location,
                Name = firstRest.Name
            };
            Location currentLocation = restaurant.Location;
            foreach (Location loc in dbLocations)
            {
                if (loc != currentLocation)
                {
                    restaurant.Location = loc;
                    break;
                }
            }
            int maxId = dbRestaurants.Max(r => r.Id);
            int restaurantsCount = dbRestaurants.Count();

            // Act
            IHttpActionResult result = controller.PostRestaurant(restaurant).Result;
            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<Restaurant>));
            var contentResult = result as CreatedAtRouteNegotiatedContentResult<Restaurant>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.Id > 0);
            Assert.AreEqual(restaurant.Name, contentResult.Content.Name);
            Assert.IsNotNull(contentResult.Content.Location);
            Assert.AreEqual(restaurant.Location.Id, contentResult.Content.Location.Id);
            Assert.AreEqual(restaurant.Location.City, contentResult.Content.Location.City);
            Assert.AreEqual(restaurant.Location.State, contentResult.Content.Location.State);
            Assert.AreEqual(restaurantsCount, dbRestaurants.Count());
            Assert.AreEqual(contentResult.Content.Id, dbRestaurants.First().Id);
            Assert.AreEqual(dbRestaurants.First().Location, restaurant.Location);
        }

        private void PostRestaurantWithBadData(Restaurant restaurant)
        {
            // Arrange
            RestaurantsController controller = new RestaurantsController(_locationRepo.Object, _restaurantRepo.Object, _reviewRepo.Object);

            // Act
            IHttpActionResult result = controller.PostRestaurant(restaurant).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }
    }
}
