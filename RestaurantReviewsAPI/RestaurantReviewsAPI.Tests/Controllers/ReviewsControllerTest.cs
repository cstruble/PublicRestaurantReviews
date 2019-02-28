using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
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
    public class ReviewsControllerTest
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();
        static Mock<IRestaurantRepository> _restaurantRepo = new Mock<IRestaurantRepository>();
        static Mock<ILocationRepository> _locationRepo = new Mock<ILocationRepository>();
        static Mock<IReviewRepository> _reviewRepo = new Mock<IReviewRepository>();
        static Mock<IUserRepository> _userRepo = new Mock<IUserRepository>();
        static IList<Location> dbLocations;
        static IList<Restaurant> dbRestaurants;
        static IList<Review> dbReviews;
        static IList<ApplicationUser> dbUsers;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _reviewRepo.Setup(lr => lr.GetReview(It.IsAny<int>())).Returns((int id) => {
                Review rev = dbReviews.SingleOrDefault(r => r.Id == id);
                var taskSource = new TaskCompletionSource<Review>();
                taskSource.SetResult(rev);
                return taskSource.Task;
            });
            _reviewRepo.Setup(rr => rr.GetReviewsByUser(It.IsAny<string>())).Returns((string id) =>
            {
                return dbReviews.Where(r => r.User.Id.Equals(id)).ToList();
            });
            _reviewRepo.Setup(rr => rr.GetReviewOfRestaurantForUser(It.IsAny<ApplicationUser>(), It.IsAny<Restaurant>()))
                .Returns((ApplicationUser user, Restaurant restaurant) =>
                {
                    return dbReviews.SingleOrDefault(r => r.User == user && r.Restaurant == restaurant);
                });
            _reviewRepo.Setup(rr => rr.Add(It.IsAny<Review>())).Returns((Review rev) =>
            {
                int maxId = dbReviews.Max(r => r.Id);
                rev.Id = ++maxId;
                dbReviews.Add(rev);
                var taskSource = new TaskCompletionSource<Review>();
                taskSource.SetResult(rev);
                return taskSource.Task;
            });
            _reviewRepo.Setup(rr => rr.Update(It.IsAny<int>(), It.IsAny<Review>())).Returns((int id, Review rev) =>
            {
                for (var i = 0; i < dbReviews.Count(); i++)
                {
                    if (dbReviews[i].Id == id)
                    {
                        dbReviews[i].Description = rev.Description;
                        dbReviews[i].Stars = rev.Stars;
                        return true;
                    }
                }
                return false;
            });

            _restaurantRepo.Setup(restR => restR.GetRestaurant(It.IsAny<int>())).Returns((int id) => dbRestaurants.SingleOrDefault(r => r.Id == id));
            _userRepo.Setup(ur => ur.GetUserByUserId(It.IsAny<string>())).Returns((string id) =>
            {
                ApplicationUser user = dbUsers.SingleOrDefault(u => u.Id.Equals(id));
                if (user == null)
                    user = dbUsers.Last();
                return user;
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

            ApplicationUser testUser = new ApplicationUser { UserName = "testUser@test.org", Email = "testUser@test.org", EmailConfirmed = true, Id="333yyyyyy333" };
            ApplicationUser anotherTestUser = new ApplicationUser { UserName = "anotherUser@test.org", Email = "anotherUser@test.org", EmailConfirmed = true, Id="94949494949llll" };
            ApplicationUser adminUser = new ApplicationUser { UserName = "admin@admin.org", Email = "admin@admin.org", EmailConfirmed = true, Id="alalalalalalal" };
            dbUsers = new List<ApplicationUser> { testUser, anotherTestUser, adminUser };

            dbReviews = new List<Review>();
            dbReviews.Add(new Review
            {
                Id = 1,
                Description = "this is a test description",
                Restaurant = butcherAndRye,
                Stars = 4,
                User = testUser
            });
            dbReviews.Add(new Review
            {
                Id = 2,
                Description = "this is only a test description",
                Restaurant = walnut,
                Stars = 4,
                User = testUser
            });
            dbReviews.Add(new Review
            {
                Id = 3,
                Description = "this is yet another test description",
                Restaurant = brgr,
                Stars = 3,
                User = anotherTestUser
            });
            dbReviews.Add(new Review
            {
                Id = 4,
                Description = "no good at all",
                Restaurant = carlton,
                Stars = 1,
                User = adminUser
            });
        }

        [TestMethod]
        public void GetReviews()
        {
            _reviewRepo.Setup(revr => revr.GetReviews()).Returns(dbReviews);
            ReviewsController controller = new ReviewsController(_restaurantRepo.Object, _reviewRepo.Object, _locationRepo.Object, _userRepo.Object);

            // Act
            IHttpActionResult result = controller.GetReviews();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IList<ReviewDTO>>));
            var contentResult = result as OkNegotiatedContentResult<IList<ReviewDTO>>;
            Assert.IsNotNull(contentResult);
            IList<ReviewDTO> revs = contentResult.Content;
            Assert.IsNotNull(revs);
            Assert.AreEqual(dbReviews.Count(), revs.Count());
        }

        [TestMethod]
        public async Task GetReview_valid()
        {
            // Arrange
            ReviewsController controller = new ReviewsController(_restaurantRepo.Object, _reviewRepo.Object, _locationRepo.Object, _userRepo.Object);
            Review review = dbReviews.Last();
            _reviewRepo.Setup(lr => lr.GetReview(It.IsAny<int>())).Returns((int id) => {
                Review rev = dbReviews.SingleOrDefault(r => r.Id == id);
                var taskSource = new TaskCompletionSource<Review>();
                taskSource.SetResult(rev);
                return taskSource.Task;
                });

            // Act
            IHttpActionResult result = await controller.GetReview(review.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ReviewDTO>));
            var contentResult = result as OkNegotiatedContentResult<ReviewDTO>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(review.Id, contentResult.Content.Id);
            Assert.AreEqual(review.Description, contentResult.Content.Description);
            Assert.AreEqual(review.Stars, contentResult.Content.Stars);
            Assert.IsNotNull(review.Restaurant);
            Assert.AreEqual(review.Restaurant.Id, contentResult.Content.Restaurant.Id);
        }

        [TestMethod]
        public async Task GetReview_invalid()
        {
            // Arrange
            ReviewsController controller = new ReviewsController(_restaurantRepo.Object, _reviewRepo.Object, _locationRepo.Object, _userRepo.Object);

            // Act
            IHttpActionResult result = await controller.GetReview(-1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void ReviewsByUser_valid()
        {
            // Arrange
            ReviewsController controller = new ReviewsController(_restaurantRepo.Object, _reviewRepo.Object, _locationRepo.Object, _userRepo.Object);
            Review review = dbReviews.Last();
            IList<Review> revsByUser = dbReviews.Where(r => r.User.Id.Equals(review.User.Id)).ToList();

            // Act
            IHttpActionResult result = controller.ReviewsByUser(review.User.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IList<ReviewDTO>>));
            var contentResult = result as OkNegotiatedContentResult<IList<ReviewDTO>>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(revsByUser.Count(), contentResult.Content.Count());
            foreach (ReviewDTO rev in contentResult.Content)
            {
                Assert.AreEqual(review.User.Id, rev.User.Id);
            }
        }

        [TestMethod]
        public void ReviewsByUser_doesntExist()
        {
            // Arrange
            ReviewsController controller = new ReviewsController(_restaurantRepo.Object, _reviewRepo.Object, _locationRepo.Object, _userRepo.Object);

            // Act
            IHttpActionResult result = controller.ReviewsByUser("thisIddoesntexist");

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IList<ReviewDTO>>));
            var contentResult = result as OkNegotiatedContentResult<IList<ReviewDTO>>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(0, contentResult.Content.Count());
        }

        [TestMethod]
        public void ReviewsByUser_invalid()
        {
            // Arrange
            ReviewsController controller = new ReviewsController(_restaurantRepo.Object, _reviewRepo.Object, _locationRepo.Object, _userRepo.Object);

            // Act
            IHttpActionResult result = controller.ReviewsByUser("");

            // Assert
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public async Task PostReview_NewReview()
        {
            // Arrange
            Restaurant restaurant = dbRestaurants.First();
            ApplicationUser user = dbUsers.Last();

            Review review = new Review
            {
                Description = "a new test description",
                Restaurant = restaurant,
                User = user,
                Stars = 3
            };
            int maxId = dbReviews.Max(r => r.Id);
            int reviewsCount = dbReviews.Count();
            ReviewsController controller = new ReviewsController(_restaurantRepo.Object, _reviewRepo.Object, _locationRepo.Object, _userRepo.Object);

            // Act
            IHttpActionResult result = await controller.PostReview(review);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<ReviewDTO>));
            var contentResult = result as CreatedAtRouteNegotiatedContentResult<ReviewDTO>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.Id > 0);
            Assert.AreEqual(reviewsCount + 1, dbReviews.Count());
            Assert.AreEqual(contentResult.Content.Id, maxId + 1);
        }

        [TestMethod]
        public async Task PostReview_UpdateReview()
        {
            // Arrange
            Review revToUpd = dbReviews.First();
            revToUpd.Description = "this was not the original description";
            int maxId = dbReviews.Max(r => r.Id);
            int reviewsCount = dbReviews.Count();
            ReviewsController controller = new ReviewsController(_restaurantRepo.Object, _reviewRepo.Object, _locationRepo.Object, _userRepo.Object);

            // Act
            IHttpActionResult result = await controller.PostReview(revToUpd);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<ReviewDTO>));
            var contentResult = result as CreatedAtRouteNegotiatedContentResult<ReviewDTO>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.Id > 0);
            Assert.AreEqual(reviewsCount, dbReviews.Count());
            Assert.AreEqual(contentResult.Content.Id, revToUpd.Id);
        }

        [TestMethod]
        public void PostReview_Empty()
        {
            PostWithBadData(new Review { });
        }

        [TestMethod]
        public void PostReview_InvalidData_NoRestaurant()
        {
            Review review = dbReviews.First();
            Review invalidReview = new Review
            {
                Description = review.Description,
                Stars = review.Stars,
                User = review.User,
                Id = review.Id
            };
            PostWithBadData(invalidReview);
        }

        [TestMethod]
        public void PostReview_InvalidData_NoUser()
        {
            Review review = dbReviews.First();
            Review invalidReview = new Review
            {
                Description = review.Description,
                Stars = review.Stars,
                Restaurant = review.Restaurant,
                Id = review.Id
            };
            PostWithBadData(invalidReview);
        }

        [TestMethod]
        public void PostReview_InvalidData_NoDescription()
        {
            Review review = dbReviews.First();
            Review invalidReview = new Review
            {
                Stars = review.Stars,
                Restaurant = review.Restaurant,
                User = review.User,
                Id = review.Id
            };
            PostWithBadData(invalidReview);
        }

        [TestMethod]
        public void PostReview_InvalidData_NoStars()
        {
            Review review = dbReviews.First();
            Review invalidReview = new Review
            {
                Description = review.Description,
                Restaurant = review.Restaurant,
                User = review.User,
                Id = review.Id
            };
            PostWithBadData(invalidReview);
        }

        private void PostWithBadData(Review review)
        {
            // Arrange
            ReviewsController controller = new ReviewsController(_restaurantRepo.Object, _reviewRepo.Object, _locationRepo.Object, _userRepo.Object);

            // Act
            IHttpActionResult result = controller.PostReview(review).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }
    }
}
