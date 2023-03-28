﻿using REST_API.Entities;

namespace REST_API.Repositories.Contracts
{
    public interface IUserRepository
    {
        Task<User> AddUser(User user);
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
        Task<User> GetUser(string username);
        Task<User> UpdateUser(User user);
        Task DeleteUser(int id);
    }
}
