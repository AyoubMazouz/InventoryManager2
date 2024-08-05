using InventoryManager2.Models;
using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Category? Parent { get; set; }

        public ICollection<Category>? Children { get; set; }

        public ICollection<Item>? Items { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CreateUpdateCategoryVM
    {
        [Required(ErrorMessage = "Le champ \"Nom\" est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le champ \"Nom\" doit avoir une longueur maximale de 100 caractères.")]
        public string Name { get; set; }

        public int? ParentId { get; set; }
    }
}
