using Collection.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Data.Repositories
{
    public class CollectionTypeRepository : RepositoryBase<CollectionType>, ICollectionTypeRepository
    {
        public CollectionTypeRepository() : this(new CollectionDbContext())
        {
        }

        public CollectionTypeRepository(CollectionDbContext dbContext) : base(dbContext)
        {
        }


        public IQueryable<CollectionType> GetAllByUserID(int userID)
        {
            return DbContext.CollectionTypes.Where(x=> x.UserId == userID);
        }

        public void DeleteAllByTypeID(int typeID)
        {
            DbContext.Collections.RemoveRange(DbContext.Collections.Where(x => x.CollectionType.CollectionTypeId == typeID));
        }

        public Task<CollectionType> GetByTypeID(int typeID)
        {
            return DbContext.CollectionTypes.FirstOrDefaultAsync(x=>x.CollectionTypeId == typeID);
        }

        public List<CollectionType> GetDefaultType()
        {
            return DbContext.CollectionTypes.Where(x => x.CollectionTypeId == 44 || x.CollectionTypeId == 45).ToList();
        }

    }

    public interface ICollectionTypeRepository : IRepository<CollectionType>
    {
        IQueryable<CollectionType> GetAllByUserID(int userID);
        Task<CollectionType> GetByTypeID(int typeID);
        List<CollectionType> GetDefaultType();
        void DeleteAllByTypeID(int typeID);
    }
}
