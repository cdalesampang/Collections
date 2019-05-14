using Collection.Data;
using Collection.Data.Models;
using Collection.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Business.Services
{
    public class CollectionTypeService : ICollectionTypeService
    {
        private readonly ICollectionTypeRepository collectionTypeRepository;

        public CollectionTypeService(CollectionDbContext dbContext): this(new CollectionTypeRepository(dbContext))
        {

        }

        public CollectionTypeService(ICollectionTypeRepository collectionTypeRepository)
        {
            this.collectionTypeRepository = collectionTypeRepository;
        }

        public Task<List<CollectionType>> GetAllByUserIDAsync(int userID)
        {
            return collectionTypeRepository.GetAllByUserID(userID).ToListAsync();
        }

        public List<CollectionType> GetAllByUserID(int userID)
        {
            return collectionTypeRepository.GetAllByUserID(userID).ToList();
        }

        public Task<CollectionType> GetAllByTypeID(int typeID)
        {
            return collectionTypeRepository.GetByIdAsync(typeID);
        }

      

        public async Task Delete(int typeId)
        {
            var type = await collectionTypeRepository.GetByTypeID(typeId);
            collectionTypeRepository.Delete(type);
            collectionTypeRepository.DeleteAllByTypeID(typeId);
        }

        public void Create(CollectionType type)
        {
            collectionTypeRepository.Create(type);
        }

        public List<CollectionType> GetDefaultType()
        {
            return collectionTypeRepository.GetDefaultType();
        }
    }
    public interface ICollectionTypeService
    {
        Task<List<CollectionType>> GetAllByUserIDAsync(int userID);
        List<CollectionType> GetAllByUserID(int userID);
        Task<CollectionType> GetAllByTypeID(int typeID);
        List<CollectionType> GetDefaultType();
        void Create(CollectionType type);
        Task Delete(int typeId);
    }
}
