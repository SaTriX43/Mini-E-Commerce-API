using System.ComponentModel.DataAnnotations;

namespace Mini_E_Commerce_API.Models
{
    public class Carrito
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public Usuario Usuario { get; set; }

        public ICollection<CarritoItem> Items { get; set; } = new List<CarritoItem>();
    }
}
