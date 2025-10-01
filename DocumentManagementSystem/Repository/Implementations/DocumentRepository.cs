using DocumentManagementSystem.Models;
using DocumentManagementSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public Document GetDocumentWithOwner(int id)
        {
            return _context.Documents
                .Include(d => d.Owner)
                .Include(d => d.Category)
                .FirstOrDefault(d => d.Id == id);
        }

        public List<Document> GetDocumentsByOwnerId(int ownerId)
        {
            return _context.Documents
                .Include(d => d.Owner)
                .Include(d => d.Category)
                .Where(d => d.OwnerId == ownerId)
                .ToList();
        }
    }
}
