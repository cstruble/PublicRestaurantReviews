using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RestaurantReviewsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantReviewsAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        protected ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext context)
        {
            _db = context;
        }

        public void SetContext(ApplicationDbContext context)
        {
            this._db = context;
        }

        public ApplicationUser GetUserByUserId(string userId)
        {
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this._db));
            return UserManager.FindById(userId);
        }
    }
}