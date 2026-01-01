using Mini_E_Commerce_API.DTOs.AutenticacionDtoCarpeta;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.Services.AutenticacionServiceCarpeta
{
    public interface IAutenticacionService
    {
        Task<Result<AutenticacionResponseDto>> RegistrarAsync(RegistroRequestDto dto);
        Task<Result<AutenticacionResponseDto>> LoginAsync(LoginRequestDto dto);
        Task<Result<AutenticacionResponseDto>> RefreshToken(RefreshTokenRenovarDto token);
        Task<Result> Logout(RefreshTokenRenovarDto token);
    }

}
