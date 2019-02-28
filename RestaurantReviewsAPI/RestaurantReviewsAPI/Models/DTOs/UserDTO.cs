using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantReviewsAPI.Models.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public UserDTO(ApplicationUser user)
        {
            this.Id = user.Id;
            this.UserName = user.UserName;
            this.Email = user.Email;
        }
    }
}