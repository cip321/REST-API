using Microsoft.EntityFrameworkCore;
using REST_API.Data;
using REST_API.Entities;
using REST_API.Repositories.Contracts;

namespace REST_API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RestApiDbContext restApiDbContext;
        public UserRepository(RestApiDbContext restApiDbContext)
        {
            this.restApiDbContext = restApiDbContext;
        }

        public async Task<User> AddUser(User user)
        {
            if (user is null)
            {
                throw new InvalidOperationException("User cannot be null");
            }
            await restApiDbContext.Users.AddAsync(user);
            await restApiDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await restApiDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<User> GetUser(string email)
        {
            var user = await restApiDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await restApiDbContext.Users.ToListAsync();
            return users;
        }

        public async Task<User> UpdateUser(User user)
        {
            if (user is null)
                throw new InvalidOperationException("User cannot be null");
            restApiDbContext.Users.Update(user);
            await restApiDbContext.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUser(int id)
        {
            var user = await restApiDbContext.Users.FindAsync(id);
            if (user is null)
                throw new InvalidOperationException("User not found");
            restApiDbContext.Users.Remove(user);
            await restApiDbContext.SaveChangesAsync();
        }
    }
}
