using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RestaurantReviewsAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace RestaurantReviewsAPI.Migrations
{
    public static class SeedData
    {
        public static void Initialize()
        {
            using (var context = new ApplicationDbContext()) {
                Location pittsburgh = new Location() { Id = 1, City = "Pittsburgh", State = "PA" };
            Location cranberry = new Location() { Id = 2, City = "Cranberry", State = "PA" };
            Location wexford = new Location() { Id = 3, City = "Wexford", State = "PA" };

            context.Locations.AddOrUpdate(x => x.Id, pittsburgh, cranberry, wexford);
                Save(context);

            string adminRole = "Admin";
            string generalUserRole = "GeneralUser";

            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists(adminRole))
            {
                roleManager.Create(new IdentityRole(adminRole));
            }
            if (!roleManager.RoleExists(generalUserRole))
            {
                roleManager.Create(new IdentityRole(generalUserRole));
            }

            ApplicationUser testUser = new ApplicationUser { UserName = "testUser@test.org", Email = "testUser@test.org", EmailConfirmed = true };
            ApplicationUser anotherTestUser = new ApplicationUser { UserName = "anotherUser@test.org", Email = "anotherUser@test.org", EmailConfirmed = true };
            ApplicationUser adminUser = new ApplicationUser { UserName = "admin@admin.org", Email = "admin@admin.org", EmailConfirmed = true };
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            testUser = AddOrUpdateUser(userManager, testUser, generalUserRole, "Password123");
            adminUser = AddOrUpdateUser(userManager, adminUser, adminRole, "Admin123");
            anotherTestUser = AddOrUpdateUser(userManager, anotherTestUser, generalUserRole, "Another123");
                Save(context);

                Restaurant butcherAndRye = CreateRestaurant(context, "Butcher and The Rye", pittsburgh);
            Restaurant sienna = CreateRestaurant(context, "Sienna Mercado", pittsburgh);
            Restaurant carlton = CreateRestaurant(context, "The Carlton Restaurant", pittsburgh);
            Restaurant ichiban = CreateRestaurant(context, "Ichiban Steak House  & Sushi Bar", cranberry);
            Restaurant brgr = CreateRestaurant(context, "BRGR", cranberry);
            Restaurant walnut = CreateRestaurant(context, "Walnut Grill", wexford);
                Save(context);

                int maxReviewId = 1;
            if (context.Reviews != null && context.Reviews.Count() > 0)
            {
                maxReviewId = context.Reviews.Max(r => r.Id);
            }
                Review review = context.Reviews.Where(r => r.UserId == testUser.Id && r.RestaurantId == carlton.Id).SingleOrDefault();
                if (review == null)
                {
                    review = new Review() { Id = maxReviewId, RestaurantId = carlton.Id, UserId = testUser.Id, Stars = 3, Description = "I thought it was good." };
                    context.Reviews.AddOrUpdate(review);
                }
                Review anotherReview = context.Reviews.Where(r => r.UserId == anotherTestUser.Id && r.RestaurantId == carlton.Id).SingleOrDefault();
                if (anotherReview == null)
                {
                    anotherReview = new Review() { Id = ++maxReviewId, RestaurantId = carlton.Id, UserId = anotherTestUser.Id, Stars = 4, Description = "Really good food, decent service." };
                    context.Reviews.AddOrUpdate(anotherReview);
                }
                    Save(context);
            }
        }

        private static void Save(ApplicationDbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbevex)
            {
                Exception raise = dbevex;
                foreach (var validationErrors in dbevex.EntityValidationErrors)
                {
                    foreach (var error in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), error.ErrorMessage);
                        System.Console.Write("message = " + message);
                    }
                }
            }

        }
        private static Restaurant CreateRestaurant(ApplicationDbContext context, string restaurantName, Location location)
        {
            if (location == null || context == null || String.IsNullOrWhiteSpace(restaurantName))
            {
                return null;
            }

            int maxId = 1;
            Restaurant dbRest = null;
            if (context.Restaurants != null && context.Restaurants.Count() > 0)
            {
                dbRest = context.Restaurants.Where(r => r.Name.Equals(restaurantName, StringComparison.OrdinalIgnoreCase) && r.Location.Id == location.Id).SingleOrDefault();
                if (dbRest == null)
                {
                    maxId = context.Restaurants.Max(r => r.Id);
                }
            }

            if (dbRest == null)
            {
                dbRest = new Restaurant() { Id = ++maxId, Name = restaurantName, Location = location };
            }

            context.Restaurants.AddOrUpdate(dbRest);
            return dbRest;
        }
        private static ApplicationUser AddOrUpdateUser(ApplicationUserManager userManager, ApplicationUser user, string roleName, string password = null)
        {
            if (userManager == null || user == null || String.IsNullOrWhiteSpace(roleName))
            {
                return null;
            }
            if (String.IsNullOrWhiteSpace(password))
            {
                password = "Password123";
            }

            IdentityResult result;
            ApplicationUser dbUser = userManager.FindByEmail(user.Email);
            if (dbUser == null)
            {
                result = userManager.Create(user, password);
                dbUser = userManager.FindByEmail(user.Email);
            } else
            {
                dbUser.EmailConfirmed = user.EmailConfirmed;
                dbUser.UserName = user.UserName;
                result = userManager.Update(dbUser);
            }

            if (result.Succeeded)
            {
                IList<string> roles = userManager.GetRoles(dbUser.Id);
                if (!roles.Contains(roleName))
                {
                    userManager.AddToRole(dbUser.Id, roleName);
                }
            }

            return dbUser;

        }
    }
}