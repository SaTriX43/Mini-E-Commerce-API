namespace Mini_E_Commerce_API.Models
{
    public class OrdenDetalle
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public Orden Orden { get; set; }
        public Producto Producto { get; set; }
    }
}
