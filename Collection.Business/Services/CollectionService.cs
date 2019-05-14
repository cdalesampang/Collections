using Collection.Data;
using Collection.Data.Models;
using Collection.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Business.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository collectionRepository;
        public CollectionService(CollectionDbContext dbContext) : this(new CollectionRepository(dbContext))
        {

        }
        public CollectionService(ICollectionRepository collectionRepository)
        {
            this.collectionRepository = collectionRepository;
        }

        public Task<List<Data.Models.Collection>> GetAllAsync(int userId)
        {
            return collectionRepository.GetAll(userId).ToListAsync();
        }

        public Task<List<Data.Models.Collection>> GetByMonthAndYearAsync(int userId, int year, int month)
        {
            return collectionRepository.GetByMonthAndYearAsync(userId,year,month).ToListAsync();
        }

        public List<Data.Models.Collection> GetByMonthAndYear(int userId, int year, int month)
        {
            return collectionRepository.GetByMonthAndYearAsync(userId, year, month).ToList();
        }


        public Task<List<Data.Models.Collection>> GetAllByDateAsync(int userId, DateTime collectionDate)
        {
            return collectionRepository.GetAllByDate(userId,collectionDate).ToListAsync();
        }
        public async Task Delete(int id)
        {
            var deleteCollection = await collectionRepository.GetByIdAsync(id);
            collectionRepository.Delete(deleteCollection);
        }
        public void Create(Data.Models.Collection collection)
        {
            collectionRepository.Create(collection);
        }


    }

    public interface ICollectionService
    {
        Task<List<Data.Models.Collection>> GetAllAsync(int userId);
        Task<List<Data.Models.Collection>> GetByMonthAndYearAsync(int userId, int year, int month);
        Task<List<Data.Models.Collection>> GetAllByDateAsync(int userId, DateTime collectionDate);
        Task Delete(int id);
        void Create(Data.Models.Collection collection);
        List<Data.Models.Collection> GetByMonthAndYear(int userId, int year, int month);
    }
}
