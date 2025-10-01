using DocumentManagementSystem.Models;
using System.Collections.Generic;

namespace DocumentManagementSystem.Repository.Interfaces
{
    public interface INotificationRepository
    {
        void Add(Notification notification);
        List<Notification> GetAllWithUser(int id);

        void RemoveByUserAndDocument(int userId, int documentId);

        void RemoveByDocument(int documentId);
    }
}
