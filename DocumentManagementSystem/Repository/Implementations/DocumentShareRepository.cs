using DocumentManagementSystem.Models;
using DocumentManagementSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DocumentManagementSystem.Repository.Implementations
{
    public class DocumentShareRepository : IDocumentShareRepository
    {
        private readonly AppDbContext _context;

        public DocumentShareRepository(AppDbContext context)
        {
            _context = context;
        }

        // this method return all the document shrare and inculde the related document , share with user and admin.
        public List<DocumentShare> GetAll()
        {
            return _context.DocumentShares
                .Include(ds => ds.Document)
                .Include(ds => ds.ShareWithUser)
                .Include(ds => ds.ShareByUser)
                .ToList();
        }

        

        public void Add(DocumentShare documentShare)
        {
            _context.DocumentShares.Add(documentShare);
            _context.SaveChanges();
        }

     

        public void Delete(int id)
        {
            var documentShare = _context.DocumentShares.FirstOrDefault(ds => ds.Id == id);
            if (documentShare != null)
            {
                _context.DocumentShares.Remove(documentShare);
                _context.SaveChanges();
            }
        }


        // this methods gives us list of document shere with the user id and active user.
        // then add the document which is shered and fetch the document and fetch shere by user from the DocumentShere.
        public List<DocumentShare> GetSharedWithUser(int userId)
        {
            return _context.DocumentShares
                .Include(ds => ds.Document)
                .ThenInclude(d => d.Owner)
                .Include(ds => ds.ShareByUser)
                .Where(ds => ds.ShareWithUserId == userId && ds.IsActive)
                .ToList();
        }

        public List<DocumentShare> GetSharedByUser(int userId)
        {
            return _context.DocumentShares
                .Include(ds => ds.Document)
                    .ThenInclude(d => d.Owner)
                .Include(ds => ds.Document)
                    .ThenInclude(d => d.Category)
                .Include(ds => ds.ShareWithUser)
                .Where(ds => ds.ShareByUserId == userId && ds.IsActive)
                .ToList();
        }

        public bool IsDocumentSharedWithUser(int documentId, int userId)
        {
            return _context.DocumentShares
                .Any(ds => ds.DocumentId == documentId && ds.ShareWithUserId == userId && ds.IsActive);
        }


        public List<User> GetUsersForSharing(int documentOwnerId)
        {
            return _context.Users
                .Where(u => u.Id != documentOwnerId)
                .ToList();
        }
    }
}
