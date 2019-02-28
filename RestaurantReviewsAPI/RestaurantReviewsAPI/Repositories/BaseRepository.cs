using RestaurantReviewsAPI.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantReviewsAPI.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : Entity
    {
        protected ApplicationDbContext _db;

        public BaseRepository(ApplicationDbContext context)
        {
            _db = context;
        }

        public void SetContext(ApplicationDbContext context)
        {
            this._db = context;
        }

        public T Get(int id)
        {
            return (T)_db.Set(typeof(T)).Find(id);
        }

        public bool Update(int id, T entity)
        {
            if (id != entity.Id)
            {
                throw new ArgumentException("Entity and Id do not match");
            }

            _db.Entry(entity).State = EntityState.Modified;

            using (DbContextTransaction tx = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.SaveChanges();
                    tx.Commit();
                }
                catch (DbUpdateConcurrencyException ducex)
                {
                    tx.Rollback();
                    if (!EntityExists(id))
                    {
                        return false;
                    }
                    else
                    {
                        throw ducex;
                    }
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw ex;
                }
            }

            return true;
        }

        public async Task<T> Add(T entity)
        {
            if (entity == null)
                return null;
            entity = (T)_db.Set(entity.GetType()).Add(entity);
            using (DbContextTransaction tx = _db.Database.BeginTransaction())
            {
                try
                {
                    await _db.SaveChangesAsync();
                    tx.Commit();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw ex;
                }
            }
            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            T entity = (T)_db.Set(typeof(T)).Find(id);
            if (entity == null)
            {
                return false;
            }

            _db.Set(typeof(T)).Remove(entity);

            using (DbContextTransaction tx = _db.Database.BeginTransaction())
            {
                try
                {
                    await _db.SaveChangesAsync();
                    tx.Commit();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw ex;
                }
            }

            return true;
        }

        private bool EntityExists(int id)
        {
            return ((IQueryable<T>)_db.Set(typeof(T))).Count(e => e.Id == id) > 0;
        }

    }
}