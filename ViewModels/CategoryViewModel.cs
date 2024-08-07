using InventoryManager2.Models;
using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.ViewModels
{
    public class CategoryVM
    {
        [Display(Name = "Identifiant")]
        public int Id { get; set; }

        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Display(Name = "Catégorie parent")]
        public Category? Parent { get; set; }

        [Display(Name = "Catégorie parent")]
        public string? ParentName { get; set; }

        [Display(Name = "Sous-catégories")]
        public ICollection<Category>? Children { get; set; }

        [Display(Name = "Articles")]
        public ICollection<Item>? Items { get; set; }

        [Display(Name = "Créé le")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Mis à jour le")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CreateUpdateCategoryVM
    {
        [Display(Name = "Nom")]
        [Required(ErrorMessage = "Le champ \"Nom\" est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le champ \"Nom\" doit avoir une longueur maximale de 100 caractères.")]
        public string Name { get; set; }

        [Display(Name = "Parent")]
        public int? ParentId { get; set; }
    }
}
