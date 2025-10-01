using DocumentManagementSystem.Models;
using DocumentManagementSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DocumentManagementSystem.Repository.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;
        public NotificationRepository(AppDbContext context) { 
            this._context = context;
        }

        void INotificationRepository.Add(Notification notification) { 
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        List<Notification> INotificationRepository.GetAllWithUser(int id) {
            return _context.Notifications
                .Include(u => u.User)
                .Where(u => u.UserId == id)
                .ToList();
        }
        void INotificationRepository.RemoveByUserAndDocument(int userId, int documentId)
        {
            var notifications = _context.Notifications
                .Where(n => n.UserId == userId && n.documentId == documentId)
                .ToList();

            if (notifications.Any())
            {
                _context.Notifications.RemoveRange(notifications);
                _context.SaveChanges();
            }
        }

        void INotificationRepository.RemoveByDocument(int documentId)
        {
            var notifications = _context.Notifications
                .Where(n => n.documentId == documentId)
                .ToList();

            if (notifications.Any())
            {
                _context.Notifications.RemoveRange(notifications);
                _context.SaveChanges();
            }
        }
    }
}
