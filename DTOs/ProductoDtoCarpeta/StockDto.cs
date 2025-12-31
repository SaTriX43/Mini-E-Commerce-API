using System.ComponentModel.DataAnnotations;

namespace Mini_E_Commerce_API.DTOs.ProductoDtoCarpeta
{
    public class StockDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
    }
}
