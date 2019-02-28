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
using RestaurantReviewsAPI.Models;
using RestaurantReviewsAPI.Models.DTOs;
using RestaurantReviewsAPI.Repositories;

namespace RestaurantReviewsAPI.Controllers
{
    [EnableCors(origins:"*", headers:"*", methods:"*")]
    [RoutePrefix("api/Restaurants")]
    public class RestaurantsController : BaseApiController
    {
        private ApplicationDbContext _db;
        private ILocationRepository _locationRepo;
        private IRestaurantRepository _restaurantRepo;
        private IReviewRepository _reviewRepo;

        public RestaurantsController(ILocationRepository locRepo, IRestaurantRepository restRepo, IReviewRepository reviewRepo)
        {
            _db = new ApplicationDbContext();
            _locationRepo = locRepo;
            _restaurantRepo = restRepo;
            _reviewRepo = reviewRepo;
        }

        // GET: api/Restaurants
        [ResponseType(typeof(IList<RestaurantDTO>))]
        public IHttpActionResult GetRestaurants()
        {
            IList<RestaurantDTO> dtos = new List<RestaurantDTO>();
            IList<Restaurant> restaurants = _restaurantRepo.GetRestaurants();
            if (restaurants == null || restaurants.Count() == 0)
                return Ok(dtos);
            foreach (Restaurant restaurant in restaurants)
            {
                dtos.Add(new RestaurantDTO(restaurant));
            }
            return Ok(dtos);
        }

        // GET: api/Restaurants/5
        [ResponseType(typeof(RestaurantDTO))]
        public IHttpActionResult GetRestaurant(int id)
        {
            Restaurant restaurant = _restaurantRepo.GetRestaurant(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return Ok(new RestaurantDTO(restaurant));
        }

        [ResponseType(typeof(RestaurantDTO))]
        [Route("addRestaurantLocation")]
        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> AddRestaurantLocation(Restaurant restaurant)
        {
            if (restaurant == null || restaurant.Location == null || String.IsNullOrWhiteSpace(restaurant.Name))
            {
                ModelState.AddModelError("Restaurant", "Invalid Restaurant Data");
                return BadRequest(ModelState);
            }

            // ensure AddOrUpdate creates a new restaurant since no corresponding database record with id=-1
            restaurant.Id = -1;

            return await PostRestaurant(restaurant);
        }

        // POST: api/Restaurants
        [ResponseType(typeof(RestaurantDTO))]
        [Authorize]
        public async Task<IHttpActionResult> PostRestaurant(Restaurant restaurant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                restaurant = await AddOrUpdateRestaurant(restaurant);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (restaurant == null)
                return InternalServerError();

            return CreatedAtRoute("DefaultApi", new { id = restaurant.Id }, restaurant);
        }

        private async Task<Restaurant> AddOrUpdateRestaurant(Restaurant restaurant)
        {
            if (restaurant == null || restaurant.Location == null || String.IsNullOrWhiteSpace(restaurant.Name))
            {
                ModelState.AddModelError("Restaurant", "Invalid Restaurant Data");
                return null;
            }

            Restaurant dbRestaurant = null;
            if (restaurant.Id > 0)
                dbRestaurant = _restaurantRepo.Get(restaurant.Id);

            Location dbLocation = _locationRepo.GetLocation(restaurant.Location.Id);
            if (dbLocation == null)
            {
                ModelState.AddModelError("Location", "Invalid Location Data");
                return null;
            }
            //else
            //{
                // we didn't find the id in the database, so search for a restaurant with the same name at the same location
                // this simple location setup (just city and state) assumes only one restaurant of a given name can exist
                // at that location.  If we included street addresses and such, would be more realistic
                Restaurant restByNameAndLoc = _restaurantRepo.GetRestaurantByNameAndLocationId(restaurant.Name, restaurant.Location.Id);
                if (restByNameAndLoc != null)
                {
                    ModelState.AddModelError("Restaurant", "Restaurant Already Exists");
                    return null;
                }
            //}
            if (dbRestaurant != null)
            {
                restaurant = new Restaurant
                {
                    Id = dbRestaurant.Id,
                    Name = dbRestaurant.Name
                };
            }
            restaurant.LocationId = dbLocation.Id;

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                _restaurantRepo.SetContext(context);
                _locationRepo.SetContext(context);

                try
                {
                    if (dbRestaurant == null)
                    {
                        restaurant = await _restaurantRepo.AddRestaurant(restaurant);
                    }
                    else if (dbRestaurant.Location.Id != restaurant.LocationId)
                    {
                        _restaurantRepo.UpdateRestaurant(dbRestaurant.Id, restaurant);
                    }
                    restaurant = _restaurantRepo.GetRestaurant(restaurant.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return restaurant;
        }

        // DELETE: api/Restaurants/5
        [ResponseType(typeof(Restaurant))]
        public async Task<IHttpActionResult> DeleteRestaurant(int id)
        {
            Restaurant restaurant = await _db.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            _db.Restaurants.Remove(restaurant);
            await _db.SaveChangesAsync();

            return Ok(restaurant);
        }

        [ResponseType(typeof(IList<Review>))]
        [Route("{restaurantId}/reviews")]
        [HttpGet]
        public IList<Review> Reviews(int restaurantId)
        {
            return _reviewRepo.GetReviewsByRestaurant(restaurantId);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool RestaurantExists(int id)
        {
            return _db.Restaurants.Count(e => e.Id == id) > 0;
        }
    }
}