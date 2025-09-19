using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.Models
{
    public class DocumentCreateViewModel
    {
        [Required(ErrorMessage = "Document title is required")]
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a file")]
        public IFormFile File { get; set; }   
    }
}
