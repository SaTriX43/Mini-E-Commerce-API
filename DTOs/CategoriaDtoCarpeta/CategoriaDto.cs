using System.ComponentModel.DataAnnotations;

namespace Mini_E_Commerce_API.DTOs.CategoriaDtoCarpeta
{
    public class CategoriaDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
