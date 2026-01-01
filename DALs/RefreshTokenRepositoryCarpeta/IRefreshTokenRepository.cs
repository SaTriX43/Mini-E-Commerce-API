using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.AutenticacionRepositoryCarpeta
{
    public interface IRefreshTokenRepository
    {
        public Task<RefreshToken> CrearRefreshTokenAsync(RefreshToken refreshToken);
        public Task<RefreshToken?> ObtenerRefreshTokenPorTokenAsync(string token);
        public Task GuardarCambiosAsync();
    }

}
