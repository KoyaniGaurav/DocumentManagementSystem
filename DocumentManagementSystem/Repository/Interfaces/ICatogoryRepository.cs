using DocumentManagementSystem.Models;
using System.Collections.Generic;

namespace DocumentManagementSystem.Repository.Interfaces
{
    public interface ICatogoryRepository
    {
        List<Category> GetAll();
        Category GetById(int id);
        Category GetByName(string name);
        void Add(Category category);
        void Update(Category category);
        void Delete(Category category);

    }
}
