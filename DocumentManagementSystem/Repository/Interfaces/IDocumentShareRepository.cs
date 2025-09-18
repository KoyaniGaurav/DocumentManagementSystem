using DocumentManagementSystem.Models;
using System.Collections.Generic;

namespace DocumentManagementSystem.Repository.Interfaces
{
    public interface IDocumentShareRepository
    {
        List<DocumentShare> GetAll();

        DocumentShare GetById(int id);

        void Add(DocumentShare documentShare);

        void Update(DocumentShare documentShare);

        void Delete(int id);

    }
}
