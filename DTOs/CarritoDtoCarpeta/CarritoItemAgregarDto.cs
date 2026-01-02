using System.ComponentModel.DataAnnotations;

namespace Mini_E_Commerce_API.DTOs.CarritoDtoCarpeta
{
    public class CarritoItemAgregarDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
