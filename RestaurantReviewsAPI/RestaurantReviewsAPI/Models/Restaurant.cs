using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantReviewsAPI.Models
{
    public class Restaurant : Entity
    {
        public string Name { get; set; }

        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

    }
}