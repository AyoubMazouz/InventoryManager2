namespace InventoryManager2.Models
{
        public class Item
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public ItemStatus Status { get; set; }
            public int ItemDetiailId { get; set; }
            public ItemDetail ItemDetail { get; set; }
            public int? CategoryId { get; set; }
            public Category? Category { get; set; }
            public int? SupplierId { get; set; }
            public Supplier? Supplier { get; set; }
            public string UserId { get; set; }
            public User User { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
            

            public enum ItemStatus
            {
                Available,
                OutOfStock,
                Discontinued,
                PendingRestock
            }
        }
}
