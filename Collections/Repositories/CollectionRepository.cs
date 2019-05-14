using Collection.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Data.Repositories
{
    public class CollectionRepository : RepositoryBase<Models.Collection> , ICollectionRepository
    {
        public CollectionRepository() : this (new CollectionDbContext())
        {

        }
        public CollectionRepository(CollectionDbContext dbContext) : base(dbContext)
        {

        }

        public IQueryable<Models.Collection> GetAll(int userId)
        {
            return DbContext.Collections.Where(x => x.UserId == userId).Include(x=>x.CollectionType);
        }

        public IQueryable<Models.Collection> GetByMonthAndYearAsync(int userId,int year, int month)
        {
            return DbContext.Collections.Where(x => x.UserId == userId && x.CollectionDate.Month == month && x.CollectionDate.Year == year).Include(x => x.CollectionType);
        }

        public IQueryable<Models.Collection> GetAllByDate(int userId, DateTime collectionDate)
        {
            return DbContext.Collections.Where(x=> x.UserId == userId && DbFunctions.TruncateTime(x.CollectionDate) == DbFunctions.TruncateTime(collectionDate.Date));
        }
        public Task<Models.Collection> GetByIdAsync(int id)
        {
            return DbContext.Collections.FirstOrDefaultAsync(x=>x.CollectionId == id);
        }
    }

    public interface ICollectionRepository : IRepository<Models.Collection>
    {
        IQueryable<Models.Collection> GetAll(int userId);
        IQueryable<Models.Collection> GetByMonthAndYearAsync(int userId, int year, int month);
        IQueryable<Models.Collection> GetAllByDate(int userId, DateTime collectionDate);
        Task<Models.Collection> GetByIdAsync(int id);
    }
}
