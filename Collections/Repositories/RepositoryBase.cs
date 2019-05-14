using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Data.Repositories
{
    public class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly DbSet<T> DbSet;
        protected readonly CollectionDbContext DbContext;

        public RepositoryBase(CollectionDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<T>();
        }

        public virtual Task<T> GetByIdAsync(object id)
        {
            return DbSet.FindAsync(id);
        }

        public IQueryable<T> GetAll()
        {
            return DbSet.AsQueryable();
        }

        public virtual void Create(T entity)
        {
            DbSet.Add(entity);
        }

        public virtual void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

    }
    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(object id);
        void Create(T entity);

        /// <summary>
        /// Does a hard delete of the entity.  Override this if you want to soft delete instead.
        /// </summary>
        void Delete(T entity);

        IQueryable<T> GetAll();
    }
}
