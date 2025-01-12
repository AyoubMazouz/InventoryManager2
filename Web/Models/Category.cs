using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        public Category? Parent { get; set; }

        public ICollection<Category>? Children { get; set; }

        public ICollection<Item>? Items { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}