using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.AutenticacionRepositoryCarpeta
{
    public interface IRefreshTokenRepository
    {
        public RefreshToken CrearRefreshTokenAsync(RefreshToken refreshToken);
        public Task<RefreshToken?> ObtenerRefreshTokenPorTokenAsync(string token);
    }

}
