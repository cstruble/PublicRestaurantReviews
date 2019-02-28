using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantReviewsAPI.Models.DTOs
{
    public class ReviewDTO : DTO
    {
        public int Stars { get; set; }
        public string Description { get; set; }
        public RestaurantDTO Restaurant { get; set; }
        public UserDTO User { get; set; }

        public ReviewDTO(Review review)
        {
            this.Id = review.Id;
            this.Stars = review.Stars;
            this.Description = review.Description;
            this.Restaurant = review.Restaurant == null ? null : new RestaurantDTO(review.Restaurant);
            this.User = review.User == null ? null : new UserDTO(review.User);
        }
    }
}