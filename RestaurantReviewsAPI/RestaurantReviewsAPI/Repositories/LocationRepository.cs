using RestaurantReviewsAPI.Models;
using RestaurantReviewsAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RestaurantReviewsAPI.Repositories
{
    public class LocationRepository : BaseRepository<Location>, ILocationRepository
    {
        public LocationRepository(ApplicationDbContext context) : base(context) { }

        public IList<Location> GetLocations()
        {
            return _db.Locations.ToList();
            //var locations = from loc in _db.Locations
            //                  select new LocationDTO
            //                  {
            //                      Id = loc.Id,
            //                      City = loc.City,
            //                      State = loc.State
            //                  };
            //return locations;
        }

        public Location GetLocation(int id)
        {
            return base.Get(id);
            //LocationDTO dto = null;
            //Location location = base.Get(id);
            //if (location != null)
            //{
            //    dto = new LocationDTO
            //    {
            //        Id = location.Id,
            //        City = location.City,
            //        State = location.State
            //    };
            //}
            //return dto;
        }

        //public bool Update(int id, Location location)
        //{
            //if (id != location.Id)
            //{
            //    return false;
            //}

            //_db.Entry(location).State = EntityState.Modified;

            //try
            //{
            //    _db.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException ducex)
            //{
            //    if (!LocationExists(id))
            //    {
            //        return false;
            //    }
            //    else
            //    {
            //        throw ducex;
            //    }
            //}

            //return true;
        //}

        public async Task<Location> AddLocation(Location location)
        {
            return await base.Add(location);
            //location = await base.Add(location);
            //return location;
            //if (location == null)
            //    return null;

            //LocationDTO dto = new LocationDTO
            //{
            //    Id = location.Id,
            //    City = location.City,
            //    State = location.State
            //};

            //return dto;
        }

        //public bool DeleteLocation(int id)
        //{
        //    Location location = _db.Locations.Find(id);
        //    return base.Delete(location);
            //if (location == null)
            //{
            //    return false;
            //}

            //_db.Locations.Remove(location);

            //try
            //{
            //    _db.SaveChanges();
            //} catch (Exception ex) {
            //    throw ex;
            //}

            //return true;
        //}

        //private bool LocationExists(int id)
        //{
        //    return _db.Locations.Count(e => e.Id == id) > 0;
        //}
    }
}