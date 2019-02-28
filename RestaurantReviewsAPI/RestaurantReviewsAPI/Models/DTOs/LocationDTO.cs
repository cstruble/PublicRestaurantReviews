using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantReviewsAPI.Models.DTOs
{
    public class LocationDTO : DTO
    {
        public string City { get; set; }
        public string State { get; set; }

        public LocationDTO(Location location)
        {
            this.City = location.City;
            this.State = location.State;
            this.Id = location.Id;
        }
    }
}