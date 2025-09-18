using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
