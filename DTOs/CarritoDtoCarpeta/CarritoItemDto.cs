namespace Mini_E_Commerce_API.DTOs.CarritoDtoCarpeta
{
    public class CarritoItemDto
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quatity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }
}
