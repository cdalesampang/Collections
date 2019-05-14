using Collection.Common.Utilities;
using Collection.Data;
using Collection.Data.Models;
using Collection.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Data.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private Utility utility;
        public UserRepository() : this(new CollectionDbContext())
        {
            utility = new Utility();
        }

        public UserRepository(CollectionDbContext dbContext) : base(dbContext)
        {
            utility = new Utility();
        }

        public Task<User> GetByIdAsync(int id)
        {
            return DbContext.Users.Where(x => x.UserId == id).FirstOrDefaultAsync();
        }



        public async Task Update(int userId, User user)
        {
            User oldUser = await GetByIdAsync(userId);
            oldUser.Name = user.Name;
            oldUser.Email = user.Email;
            oldUser.ActiveDate = user.ActiveDate;
            if (!user.Password.Equals("*******"))
            {
                user.Password = utility.Encryption(user.Password);
                oldUser.Password = user.Password;
            }
            
           
        }
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByIdAsync(int id);
        Task Update(int userId, User user);
    }
}
