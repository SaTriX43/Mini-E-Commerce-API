using Mini_E_Commerce_API.Models.Enums;

namespace Mini_E_Commerce_API.DTOs.OrdenDtoCarpeta
{
    public class OrdenDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public StatusOrden Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<OrdenDetallesDto> OrdenDetallesDtos { get; set; }
    }
}
