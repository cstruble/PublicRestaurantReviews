using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.Owin;
using Owin;
using RestaurantReviewsAPI.Migrations;
using RestaurantReviewsAPI.Repositories;
using RestaurantReviewsAPI.Resolver;
using Unity;

[assembly: OwinStartup(typeof(RestaurantReviewsAPI.Startup))]

namespace RestaurantReviewsAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            SeedData.Initialize();
        }
    }
}
