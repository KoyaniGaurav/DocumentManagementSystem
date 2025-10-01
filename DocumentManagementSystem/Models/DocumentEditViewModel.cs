using Microsoft.AspNetCore.Http;

namespace DocumentManagementSystem.Models
{
    public class DocumentEditViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }

        public int? CategoryId { get; set; }
    }
}
