using Mini_E_Commerce_API.Models.Enums;

namespace Mini_E_Commerce_API.Models
{
    public class Orden
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public StatusOrden Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Usuario Usuario { get; set; }
        public ICollection<OrdenDetalle> Detalles { get; set; } = new List<OrdenDetalle>();

    }
}
