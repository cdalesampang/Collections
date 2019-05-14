using Collection.Common.Utilities;
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
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private Utility utility;
        public UserService(CollectionDbContext dbContext): this(new UserRepository(dbContext))
        {
            utility = new Utility();
        }

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public Task<User> CheckUser(string email, string password)
        {
            password = utility.Encryption(password);
            return userRepository.GetAll().FirstOrDefaultAsync(x => x.UserName == email && x.Password == password);
        }

        public Task<User> CheckUserFB(string id,string email)
        {
            return userRepository.GetAll().FirstOrDefaultAsync(x => x.FBid == id || x.Email == email);
        }

        public Task<User> CheckUserName(string userName)
        {
            return userRepository.GetAll().FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public Task<User> CheckEmail(string email)
        {
            return userRepository.GetAll().FirstOrDefaultAsync(x => x.Email == email);
        }

        public Task<User> GetByIdAsync(int Id)
        {
            return userRepository.GetByIdAsync(Id);
        }
        public List<User> GetAll()
        {
            return userRepository.GetAll().ToList();
        }

        public Task Update(int userId, User user)
        {
            return userRepository.Update(userId,user);
        }

        public void Create(User user)
        {
            user.Password = utility.Encryption(user.Password);
            user.ActiveDate = DateTime.Now;
            userRepository.Create(user);
        }

        public async Task Delete(int id)
        {
            var user = await GetByIdAsync(id);
            userRepository.Delete(user);
        }
    }

    public interface IUserService
    {
        Task<User> CheckUser(string email, string password);
        Task<User> GetByIdAsync(int Id);
        Task Update(int userId, User user);
        void Create( User user);
        Task Delete(int id);
        List<User> GetAll();
        Task<User> CheckUserFB(string id, string email);
        Task<User> CheckUserName(string userName);
        Task<User> CheckEmail(string email);
    }
}
