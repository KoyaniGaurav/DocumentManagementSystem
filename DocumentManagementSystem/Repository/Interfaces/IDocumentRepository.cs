using System.Collections.Generic;
using DocumentManagementSystem.Models;

namespace DocumentManagementSystem.Repository.Interfaces
{
    public interface IDocumentRepository
    {
        Document GetById(int id);              
        void Add(Document document);           
        void Update(Document document);        
        void Delete(int id);                   

        // Admin-specific methods
        Document GetDocumentWithOwner(int id);
        List<Document> GetDocumentsByOwnerId(int ownerId);   
    }
}
