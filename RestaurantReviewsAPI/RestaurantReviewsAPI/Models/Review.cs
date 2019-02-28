using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace RestaurantReviewsAPI.Models
{
    public class Review : Entity
    {
        [Required]
        public int Stars { get; set; }
        [Required]
        public string Description { get; set; }

        // Foreign Key
        public int RestaurantId { get; set; }
        // Navigation property
        [Required]
        public Restaurant Restaurant { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}