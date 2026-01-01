using System.ComponentModel.DataAnnotations;

namespace Mini_E_Commerce_API.DTOs.AutenticacionDtoCarpeta
{
    public class RefreshTokenRenovarDto
    {
        [Required]
        public string Token { get; set; }
    }
}
