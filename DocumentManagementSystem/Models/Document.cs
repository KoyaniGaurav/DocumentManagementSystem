using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagementSystem.Models
{
    public class Document
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Document title is required")]
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [StringLength(260)]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        [StringLength(50)]
        public string FileType { get; set; }

        public int OwnerId { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public bool IsShared { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
    }
}
