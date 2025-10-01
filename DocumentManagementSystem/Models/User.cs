using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagementSystem.Models
{
    public enum UserRole
    {
        User = 0,
        Admin = 1
    }

    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        [StringLength(200)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.User;

        // User - Document (one - many)
        public ICollection<Document> Documents { get; set; } = new List<Document>();

        // Admin - Document (one - meny)
        public ICollection<DocumentShare> DocumentsSharedBy { get; set; } = new List<DocumentShare>();

        // User - Document (one - many)
        public ICollection<DocumentShare> DocumentsSharedWith { get; set; } = new List<DocumentShare>();

        // User - Notification (one - many)
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
