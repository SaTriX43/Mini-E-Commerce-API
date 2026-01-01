using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Mini_E_Commerce_API.DALs.AutenticacionRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.UsuariorRepositoryCarpeta;
using Mini_E_Commerce_API.DTOs.AutenticacionDtoCarpeta;
using Mini_E_Commerce_API.Models;
using Mini_E_Commerce_API.Models.Enums;
using Mini_E_Commerce_API.Services.AutenticacionServiceCarpeta;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AutenticacionService : IAutenticacionService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AutenticacionService(
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration,
        IUsuarioRepository usuarioRepository
        )
    {
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Result<AutenticacionResponseDto>> RegistrarAsync(RegistroRequestDto dto)
    {
        var usuarioExistente = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email);

        if (usuarioExistente is not null)
            return Result<AutenticacionResponseDto>.Failure("El email ya está registrado");

        var usuario = new Usuario
        {
            Email = dto.Email,
            Name = dto.Nombre,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Rol = RolUsuario.User
        };

        var usuarioCreado = await _usuarioRepository.CrearAsync(usuario);

        var refreshTokenDiasExpiracion = _configuration.GetValue<int>("Jwt:RefreshTokenDays");
        var nuevoRefreshTokenModel = new RefreshToken
        {
            UserId = usuarioCreado.Id,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false,
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDiasExpiracion)
        };

        var token = GenerarJwt(usuario);

        var refreshTokenCreado = await _refreshTokenRepository.CrearRefreshTokenAsync(nuevoRefreshTokenModel);
        return Result<AutenticacionResponseDto>.Success(new AutenticacionResponseDto
        {
            AccessToken = token,
            RefreshToken = refreshTokenCreado.Token
        });
    }

    public async Task<Result<AutenticacionResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email);

        if (usuario is null || !usuario.IsActive)
            return Result<AutenticacionResponseDto>.Failure("Credenciales inválidas");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            return Result<AutenticacionResponseDto>.Failure("Credenciales inválidas");

        var refreshTokenDiasExpiracion = _configuration.GetValue<int>("Jwt:RefreshTokenDays");
        var nuevoRefreshTokenModel = new RefreshToken
        {
            UserId = usuario.Id,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false,
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDiasExpiracion)
        };
        var token = GenerarJwt(usuario);
        var refreshTokenCreado = await _refreshTokenRepository.CrearRefreshTokenAsync(nuevoRefreshTokenModel);

        return Result<AutenticacionResponseDto>.Success(new AutenticacionResponseDto
        {
            AccessToken = token,
            RefreshToken = refreshTokenCreado.Token
        });
    }
    public async Task<Result<AutenticacionResponseDto>> RefreshToken(RefreshTokenRenovarDto token)
    {
        var tokenEncontrado = await _refreshTokenRepository.ObtenerRefreshTokenPorTokenAsync(token.Token);

        if (tokenEncontrado == null)
        {
            return Result<AutenticacionResponseDto>.Failure("Su token no puede ser null");
        }

        if(tokenEncontrado.RevokedAt != null)
        {
            return Result<AutenticacionResponseDto>.Failure("Su token fue revocado");
        }

        if (tokenEncontrado.ExpiresAt < DateTime.UtcNow)
        {
            return Result<AutenticacionResponseDto>.Failure("Su token ya expiro");
        }

        if(tokenEncontrado.IsUsed)
        {
            return Result<AutenticacionResponseDto>.Failure("Su token ya fue usado");
        }

        if(tokenEncontrado.Usuario == null)
        {
            return Result<AutenticacionResponseDto>.Failure("Su usuario debe de existir");
        }

        tokenEncontrado.IsUsed = true;
        tokenEncontrado.RevokedAt = DateTime.UtcNow;

        await _refreshTokenRepository.GuardarCambiosAsync();

        var jwt = GenerarJwt(tokenEncontrado.Usuario);
        var refreshTokenDiasExpiracion = _configuration.GetValue<int>("Jwt:RefreshTokenDays");
        var nuevoRefreshTokenModel = new RefreshToken
        {
            UserId = tokenEncontrado.Usuario.Id,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false,
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDiasExpiracion)
        };

        var refreshTokenCreado = await _refreshTokenRepository.CrearRefreshTokenAsync(nuevoRefreshTokenModel);

        return Result<AutenticacionResponseDto>.Success(new AutenticacionResponseDto
        {
            AccessToken = jwt,
            RefreshToken = refreshTokenCreado.Token
        });
    }
    public async Task<Result> Logout(RefreshTokenRenovarDto token)
    {
        var tokenEncontrado = await _refreshTokenRepository.ObtenerRefreshTokenPorTokenAsync(token.Token);

        if (tokenEncontrado == null)
        {
            return Result<AutenticacionResponseDto>.Failure("Su token no puede ser null");
        }

        if (tokenEncontrado.RevokedAt != null)
        {
            return Result<AutenticacionResponseDto>.Failure("Su token fue revocado");
        }

        if (tokenEncontrado.ExpiresAt < DateTime.UtcNow)
        {
            return Result<AutenticacionResponseDto>.Failure("Su token ya expiro");
        }

        if (tokenEncontrado.IsUsed)
        {
            return Result<AutenticacionResponseDto>.Failure("Su token ya fue usado");
        }

        if (tokenEncontrado.Usuario == null)
        {
            return Result<AutenticacionResponseDto>.Failure("Su usuario debe de existir");
        }

        tokenEncontrado.IsUsed = true;
        tokenEncontrado.RevokedAt = DateTime.UtcNow;

        await _refreshTokenRepository.GuardarCambiosAsync();

        return Result.Success();
    }
    private string GenerarJwt(Usuario usuario)
    {
        var jwt = _configuration.GetSection("Jwt");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["SecretKey"]!)
        );

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(jwt["AccessTokenMinutes"]!)
            ),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
