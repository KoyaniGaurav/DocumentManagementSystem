using DocumentManagementSystem.Models;
using DocumentManagementSystem.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DocumentManagementSystem.Repository.Implementations
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;

        public DocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Document> GetAll()
        {
            return _context.Documents.ToList();
        }

        public Document GetById(int id)
        {
            return _context.Documents.FirstOrDefault(d => d.Id == id);
        }

        public void Add(Document document)
        {
            _context.Documents.Add(document);
            _context.SaveChanges();

        }

        public void Update(Document document)
        {
            _context.Documents.Update(document);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var document = _context.Documents.FirstOrDefault(d => d.Id == id);
            if (document != null)
            {
                _context.Documents.Remove(document);
                _context.SaveChanges();
            }
        }

        // Get documents uploaded by a specific user - pending.......
        public List<Document> GetByUserId(int userId)
        {
            return _context.Documents
                           .Where(d => d.Id == userId)
                           .ToList();
        }
    }
}
