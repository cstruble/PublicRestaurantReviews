using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RestaurantReviewsAPI.Models;
using RestaurantReviewsAPI.Models.DTOs;
using RestaurantReviewsAPI.Repositories;

namespace RestaurantReviewsAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    [RoutePrefix("api/Reviews")]
    public class ReviewsController : BaseApiController
    {
        private ApplicationDbContext _db;
        private readonly IRestaurantRepository _restaurantRepo;
        private readonly IReviewRepository _reviewRepo;
        private readonly ILocationRepository _locationRepo;
        private readonly IUserRepository _userRepo;
 
        public ReviewsController(IRestaurantRepository restRepo, IReviewRepository reviewRepo, ILocationRepository locRepo, IUserRepository userRepo)
        {
            this._restaurantRepo = restRepo;
            this._reviewRepo = reviewRepo;
            this._locationRepo = locRepo;
            this._userRepo = userRepo;
            this._db = new ApplicationDbContext();
        }

        // GET: api/Reviews
        public IHttpActionResult GetReviews()
        {
            IList<Review> reviews = _reviewRepo.GetReviews();
            IList<ReviewDTO> dtos = new List<ReviewDTO>();
            foreach (Review review in reviews)
            {
                dtos.Add(new ReviewDTO(review));
            }
            return Ok(dtos);
        }

        // GET: api/Reviews/5
        [ResponseType(typeof(ReviewDTO))]
        public async Task<IHttpActionResult> GetReview(int id)
        {
            Review review = await _reviewRepo.GetReview(id);
            if (review == null)
            {
                return NotFound();
            }

            return Ok(new ReviewDTO(review));
        }

        private Review InitializeReview(Review review, ApplicationDbContext context)
        {
            if (review == null || review.Restaurant == null)
            {
                return null;
            }

            Restaurant restaurant = _restaurantRepo.GetRestaurant(review.Restaurant.Id);
            if (restaurant == null)
                return null;

            review.Restaurant = restaurant;
            review.User = _userRepo.GetUserByUserId(User.Identity.GetUserId());
            return review;
        }

        private async Task<Review> AddOrUpdateReview(Review review)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                _restaurantRepo.SetContext(context);
                _reviewRepo.SetContext(context);

                review = InitializeReview(review, context);
                if (review == null)
                {
                    ModelState.AddModelError("Restaurant", "Invalid Review Data");
                    return null;
                }

                Review dbReview = _reviewRepo.GetReviewOfRestaurantForUser(review.User, review.Restaurant);

                try
                {
                    if (dbReview == null)
                        review = await _reviewRepo.Add(review);
                    else
                    {
                        dbReview.Description = review.Description;
                        dbReview.Stars = review.Stars;
                        _reviewRepo.Update(dbReview.Id, dbReview);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return review;
        }

        [ResponseType(typeof(IList<ReviewDTO>))]
        [Route("reviewsByUser/{userId}")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult ReviewsByUser(string userId)
        {
            if (String.IsNullOrWhiteSpace(userId))
            {
                ModelState.AddModelError("User", "User ID Invalid");
                return BadRequest(ModelState);
            }
            IList<ReviewDTO> dtos = new List<ReviewDTO>();
            IList<Review> reviews = _reviewRepo.GetReviewsByUser(userId);
            foreach (Review review in reviews)
            {
                dtos.Add(new ReviewDTO(review));
            }
            return Ok(dtos);
        }

        // POST: api/Reviews
        [ResponseType(typeof(ReviewDTO))]
        public async Task<IHttpActionResult> PostReview(Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                review = await AddOrUpdateReview(review);
            } catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (review == null)
                return InternalServerError();

            return CreatedAtRoute("DefaultApi", new { id = review.Id }, new ReviewDTO(review));
        }

        // DELETE: api/Reviews/5
        [ResponseType(typeof(Review))]
        public async Task<IHttpActionResult> DeleteReview(int id)
        {
            Review review = _reviewRepo.Get(id);
            if (review == null)
            {
                return NotFound();
            }

            bool isSuccess = await _reviewRepo.Delete(id);
            if (isSuccess)
                return Ok(review);
            return InternalServerError();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}