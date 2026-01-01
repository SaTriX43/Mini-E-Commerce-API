using System.ComponentModel.DataAnnotations;

namespace Mini_E_Commerce_API.DTOs.CategoriaDtoCarpeta
{
    public class CategoriaCrearDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
