using System.Collections.Generic;
using DocumentManagementSystem.Models;

namespace DocumentManagementSystem.Repository.Interfaces
{
    public interface IDocumentRepository
    {
        List<Document> GetAll();               
        Document GetById(int id);              
        void Add(Document document);           
        void Update(Document document);        
        void Delete(int id);                   
        List<Document> GetByUserId(int userId);


    }
}
