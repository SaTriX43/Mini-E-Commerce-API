namespace Mini_E_Commerce_API.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? RevokedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
