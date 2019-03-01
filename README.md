Created a sample data set using EF via a SeedData static class upon startup.  Used Repository pattern with IdentityContext with the assumption that if this were to plug into another system, the database connection, Repositories and Entities (found in the Models package) can be swapped out to real data connections and entities.  I used SQL Server Express locally while seeding the data and subsequently testing.

Used Identity with Roles with the idea that perhaps we'd like to have Admin role'd users update the underlying data such as Restaurants and where those restaurants are located.  Didn't delve too deeply into security. Only used the Authorize attribute to protect methods but didn't get too complicated with it.  Also, added API functionality for adding new restaurants and adding new locations for existing restaurants, but didn't expand locations for similar functionality.  Stubbed out the methods for locations instead.

Created a SPA UI in Angular to test with and included the code for that here as well.  With the UI running on a separate port, had to deal with CORS.  Again, didn't go too deeply into the CORS setup, just made it work.

I've also included a few unit tests on the Controllers.  I chose Moq as the mocking package.  

Some basic assumptions that were made:
1.  There can only be one restaurant at a location, but can be multiple locations for a restaurant.  If we expand the data set for locations to include street address, this would be more realistic.  For now, the location definition is only City and State.
2.  Given multiple potential locations for a restaurant, there can be separate reviews for each individual location.
3.  A User may only review a restaurant at a location once.  The user cannot submit multiple reviews for the same location of a restaurant.  Instead, that user can edit the existing review.
4.  A User may only edit reviews which they've submitted, not reviews written by other users.
5.  A User may only delete reviews which they've submitted
6.  Restaurants and Locations cannot be deleted (can implement if desired, but not currently included)

RestaurantReviews
=================

The Problem
--------------
We are in the midst of building a mobile application that will let restaurant patrons rate the restaurant in which they are eating. As part of the build, we need to develop a web API that will accept and store the ratings and other sundry data from a publicly accessible interface. 

At a minimum the API should be able to:

1. Get a list of restaurants by city
2. Post a restaurant that is not in the database
3. Post a review for a restaurant
4. Get of a list of reviews by user
5. Delete a review

Use whatever techniques are applicable to solve the problem. This code will be part of a larger system. 
