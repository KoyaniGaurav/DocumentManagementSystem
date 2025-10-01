using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.Models
{
    public class DocumentShareViewModel
    {
        public int DocumentId { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentOwner { get; set; }
        
        [Required(ErrorMessage = "Please select at least one user to share with")]
        public List<int> SelectedUserIds { get; set; } = new List<int>();
        
        public List<User> AvailableUsers { get; set; } = new List<User>();
    }

    public class SharedDocumentViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string CategoryName { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime SharedAt { get; set; }
        public bool IsShared { get; set; } = true;
    }

    public class AdminDocumentViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public List<string> SharedWithUserNames { get; set; } = new List<string>();
        public List<User> SharedWithUsers { get; set; } = new List<User>();
    }

    public class AdminIndexViewmodel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string CategoryName { get; set; }
        public System.DateTime CreatedAt { get; set; }


    }
}
