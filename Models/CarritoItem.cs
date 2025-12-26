using System.ComponentModel.DataAnnotations;

namespace Mini_E_Commerce_API.Models
{
    public class CarritoItem
    {
        [Key]
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quatity { get; set; }
        public Carrito Carrito { get; set; }
        public Producto Producto { get; set; }
    }
}
