using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using RestaurantReviewsAPI.Models;
using RestaurantReviewsAPI.Models.DTOs;
using RestaurantReviewsAPI.Repositories;

namespace RestaurantReviewsAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LocationsController : BaseApiController
    {
        private ILocationRepository _locationRepo;
        private IRestaurantRepository _restaurantRepo;

        public LocationsController(ILocationRepository locRepo, IRestaurantRepository restRepo)
        {
            _locationRepo = locRepo;
            _restaurantRepo = restRepo;
        }

        // GET: api/Locations
        [ResponseType(typeof(IList<LocationDTO>))]
        public IHttpActionResult GetLocations()
        {
            IList<Location> locations = _locationRepo.GetLocations();
            IList<LocationDTO> dtos = new List<LocationDTO>();
            if (locations == null || locations.Count() == 0)
                return Ok(dtos);
            foreach (Location location in locations)
            {
                dtos.Add(new LocationDTO(location));
            }
            return Ok(dtos);
        }

        // GET: api/Locations/5
        [ResponseType(typeof(Location))]
        public IHttpActionResult GetLocation(int id)
        {
            Location location = _locationRepo.GetLocation(id);
            if (location == null)
            {
                return NotFound();
            }

            return Ok(new LocationDTO(location));
        }

        // PUT: api/Locations/5
        [ResponseType(typeof(void))]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PutLocation(int id, Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != location.Id)
            {
                return BadRequest();
            }

            try
            {
                if (!_locationRepo.Update(id, location))
                {
                    return NotFound();
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                    throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Locations
        [ResponseType(typeof(Location))]
        [Authorize]
        public IHttpActionResult PostLocation(Location location)
        {
            // stubbed for future inclusion
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        // DELETE: api/Locations/5
        [ResponseType(typeof(Location))]
        [Authorize]
        public IHttpActionResult DeleteLocation(int id)
        {
            // stubbed for future inclusion
            return Ok();
        }

        [ResponseType(typeof(IList<RestaurantDTO>))]
        [Route("api/locations/{locationId}/restaurants")]
        [HttpGet]
        public IHttpActionResult Restaurants(int locationId)
        {
            IList<RestaurantDTO> dtos = new List<RestaurantDTO>();
            IList<Restaurant> restaurants = _restaurantRepo.GetRestaurantsByLocation(locationId);
            if (restaurants == null || restaurants.Count() == 0)
                return Ok(dtos);
            foreach (Restaurant restaurant in restaurants)
            {
                dtos.Add(new RestaurantDTO(restaurant));
            }
            return Ok(dtos);
        }



        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}