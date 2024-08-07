using InventoryManager2.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.ViewModels
{
    public class SupplierVM
    {
        [Display(Name = "Identifiant")]
        public int Id { get; set; }

        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Display(Name = "Informations de contact")]
        public string ContactInfo { get; set; }

        [Display(Name = "Articles")]
        public ICollection<Item>? Items { get; set; }

        [Display(Name = "Créé le")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Mis à jour le")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CreateUpdateSupplierVM
    {
        [Display(Name = "Nom")]
        [Required(ErrorMessage = "Le champ \"Nom\" est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le champ \"Nom\" doit avoir une longueur maximale de 100 caractères.")]
        public string Name { get; set; }

        [Display(Name = "Coordonnées")]
        [Required(ErrorMessage = "Le champ \"Informations de contact\" est obligatoire.")]
        [StringLength(200, ErrorMessage = "Le champ \"Informations de contact\" doit avoir une longueur maximale de 200 caractères.")]
        public string ContactInfo { get; set; }
    }
}
