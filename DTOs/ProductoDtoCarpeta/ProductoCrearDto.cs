using System.ComponentModel.DataAnnotations;

namespace Mini_E_Commerce_API.DTOs.ProductoDtoCarpeta
{
    public class ProductoCrearDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock {  get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
