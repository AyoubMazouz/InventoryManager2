using Microsoft.AspNetCore.Identity;

namespace InventoryManager2.Models
{
    public class Role: IdentityRole
    {
        public string Description { get; set; }
        public ICollection<User>? Users { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt {  get; set; }

    }
}
