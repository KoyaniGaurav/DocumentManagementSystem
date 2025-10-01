using DocumentManagementSystem.Models;
using System.Collections.Generic;

namespace DocumentManagementSystem.Repository.Interfaces
{
    public interface IUserRepository
    {
        User Get(int id);
        void Add(User user);
        void Update(User user);
        User GetByEmail(string email);
        User GetWithDocuments(int id);
    }
}
