using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RestaurantReviewsAPI.Models
{
    public class Location : Entity
    {
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
    }
}