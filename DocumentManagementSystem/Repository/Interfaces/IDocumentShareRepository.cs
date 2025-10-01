using DocumentManagementSystem.Models;
using System.Collections.Generic;

namespace DocumentManagementSystem.Repository.Interfaces
{
    public interface IDocumentShareRepository
    {
        List<DocumentShare> GetAll();

        void Add(DocumentShare documentShare);


        void Delete(int id);

        // Get documents shared with a specific user
        List<DocumentShare> GetSharedWithUser(int userId);

        // Get documents shared by a specific user
        List<DocumentShare> GetSharedByUser(int userId);

        // Check if a document is already shared with a user
        bool IsDocumentSharedWithUser(int documentId, int userId);

        // Get all users except the document owner
        List<User> GetUsersForSharing(int documentOwnerId);
    }
}
