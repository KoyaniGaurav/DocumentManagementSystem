using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagementSystem.Models
{
    public class DocumentShare
    {
        public int Id { get; set; }

        public int DocumentId { get; set; }
        public Document Document { get; set; }

        public int ShareWithUserId { get; set; }
        [ForeignKey(nameof(ShareWithUserId))]
        public User ShareWithUser { get; set; }

        public int ShareByUserId { get; set; }
        [ForeignKey(nameof(ShareByUserId))]
        public User ShareByUser { get; set; }

        public DateTime SharedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
