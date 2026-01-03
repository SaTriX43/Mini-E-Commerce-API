using Microsoft.EntityFrameworkCore;
using Mini_E_Commerce_API.DALs.AutenticacionRepositoryCarpeta;
using Mini_E_Commerce_API.Models;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public RefreshToken CrearRefreshTokenAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public async Task<RefreshToken?> ObtenerRefreshTokenPorTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.Usuario)
            .FirstOrDefaultAsync(rt => rt.Token == token);
        return refreshToken;
    }
}
