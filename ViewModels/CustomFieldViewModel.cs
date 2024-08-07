using InventoryManager2.Models;
using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.ViewModels
{
    public class CustomFieldVM
    {
        [Display(Name = "Type de données")]
        public string DataType { get; set; }

        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Display(Name = "Valeur")]
        public string? Value { get; set; }
    }

    public class CreateUpdateCustomFieldVM
    {
        [Required(ErrorMessage = "Le type de données est obligatoire.")]
        [Display(Name = "Type de données")]
        public string DataType { get; set; }

        [Display(Name = "Nom")]
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Name { get; set; }

        [Display(Name = "Valeur")]
        public string? Value { get; set; }
    }
}
