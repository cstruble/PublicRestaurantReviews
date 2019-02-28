using RestaurantReviewsAPI.Models;
using RestaurantReviewsAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReviewsAPI.Repositories
{
    public interface IRestaurantRepository : IRepository<Restaurant>
    {
        IList<Restaurant> GetRestaurants();
        Restaurant GetRestaurant(int id);
        IList<Restaurant> GetRestaurantsByLocation(int id);
        Restaurant GetRestaurantByNameAndLocationId(string name, int locationId);
        Task<Restaurant> AddRestaurant(Restaurant restaurant);
        bool UpdateRestaurant(int id, Restaurant restaurant);
    }
}
