using DocumentManagementSystem.Models;
using System.Collections.Generic;

namespace DocumentManagementSystem.Repository.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User Get(int id);
        void Add(User user);
        void Update(User user);
        void Delete(int id);
        User GetByEmail(string email);
    }
}
