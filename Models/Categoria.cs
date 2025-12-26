using System.ComponentModel.DataAnnotations;

namespace Mini_E_Commerce_API.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
