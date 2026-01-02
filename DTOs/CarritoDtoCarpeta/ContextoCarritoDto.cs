using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DTOs.CarritoDtoCarpeta
{
    public class ContextoCarritoDto
    {
        public Usuario Usuario { get; set; }
        public Carrito Carrito { get; set; }
        public Producto Producto { get; set; }
        public CarritoItem? Item { get; set; }
    }
}
