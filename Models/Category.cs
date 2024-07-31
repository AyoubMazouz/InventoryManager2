using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Le champ \"Nom\" est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le champ \"Nom\" doit avoir une longueur maximale de 100 caractères.")]
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public Category? Parent { get; set; }
        public ICollection<Category>? Children { get; set; }
        public ICollection<Item>? Items { get; set; }
    }
}