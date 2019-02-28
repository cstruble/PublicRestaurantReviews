using RestaurantReviewsAPI.Models;
using RestaurantReviewsAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReviewsAPI.Repositories
{
    public interface ILocationRepository : IRepository<Location>
    {
        IList<Location> GetLocations();
        Location GetLocation(int id);
        Task<Location> AddLocation(Location location);
    }
}
