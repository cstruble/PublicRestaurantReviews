using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantReviewsAPI.Models.DTOs
{
    public class RestaurantDTO : DTO
    {
        public string Name { get; set; }
        public LocationDTO Location { get; set; }

        public RestaurantDTO(Restaurant restaurant)
        {
            this.Id = restaurant.Id;
            this.Name = restaurant.Name;
            this.Location = restaurant.Location == null ? null : new LocationDTO(restaurant.Location);
        }
    }
}