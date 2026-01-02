using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DTOs.CarritoDtoCarpeta
{
    public class CarritoDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
        public List<CarritoItemDto> Items { get; set; } = new List<CarritoItemDto>();
        public decimal Total {  get; set; }
    }
}
