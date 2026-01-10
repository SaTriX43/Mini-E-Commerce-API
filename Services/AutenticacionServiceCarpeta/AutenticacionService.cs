using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Mini_E_Commerce_API.DALs;
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
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly ILogger<AutenticacionService> _logger;

    public AutenticacionService(
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration,
        IUsuarioRepository usuarioRepository,
        IUnidadDeTrabajo unidadDeTrabajo,
        ILogger<AutenticacionService> logger

        )
    {
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
        _usuarioRepository = usuarioRepository;
        _unidadDeTrabajo = unidadDeTrabajo;
        _logger = logger;
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

        var usuarioCreado = _usuarioRepository.Crear(usuario);

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

        var refreshTokenCreado = _refreshTokenRepository.CrearRefreshTokenAsync(nuevoRefreshTokenModel);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        _logger.LogInformation(
            "Usuario registrado exitosamente. UserId={UserId} Email={Email}",
            usuarioCreado.Id,
            usuarioCreado.Email
        );

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
        {
            _logger.LogWarning("Usuario con Email {Email} fallo al iniciar sesion - credenciales invalidas", dto.Email);
            return Result<AutenticacionResponseDto>.Failure("Credenciales inválidas");
        }

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
        var refreshTokenCreado = _refreshTokenRepository.CrearRefreshTokenAsync(nuevoRefreshTokenModel);
        await _unidadDeTrabajo.GuardarCambiosAsync();
        _logger.LogInformation("Usuario con id = {UserId} se ha logeado", usuario.Id);
        return Result<AutenticacionResponseDto>.Success(new AutenticacionResponseDto
        {
            AccessToken = token,
            RefreshToken = refreshTokenCreado.Token
        });
    }
    public async Task<Result<AutenticacionResponseDto>> RefreshToken(RefreshTokenRenovarDto token)
    {
        var tokenEncontrado = await _refreshTokenRepository.ObtenerRefreshTokenPorTokenAsync(token.Token);

        var tokenTail = token.Token?.Length > 8
            ? token.Token[^8..]
            : token.Token;

        if (tokenEncontrado == null)
        {
            _logger.LogWarning("Refresh token no existe. TokenTail={TokenTail}",tokenTail);
            return Result<AutenticacionResponseDto>.Failure("Su token no puede ser null");
        }

        if(tokenEncontrado.RevokedAt != null)
        {
            _logger.LogWarning("Usuario con id {usuarioId} envio un token revocado", tokenEncontrado.UserId);
            return Result<AutenticacionResponseDto>.Failure("Su token fue revocado");
        }

        if (tokenEncontrado.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Usuario con id {usuarioId} envio un token expirado", tokenEncontrado.UserId);
            return Result<AutenticacionResponseDto>.Failure("Su token ya expiro");
        }

        if(tokenEncontrado.IsUsed)
        {
            _logger.LogWarning("Usuario con id {usuarioId} envio un token usado", tokenEncontrado.UserId);
            return Result<AutenticacionResponseDto>.Failure("Su token ya fue usado");
        }

        if(tokenEncontrado.Usuario == null)
        {
            _logger.LogWarning("No existe usuario asignado al token {tokenTail} ", tokenTail);
            return Result<AutenticacionResponseDto>.Failure("Su usuario debe de existir");
        }

        tokenEncontrado.IsUsed = true;
        tokenEncontrado.RevokedAt = DateTime.UtcNow;

        await _unidadDeTrabajo.GuardarCambiosAsync();

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

        var refreshTokenCreado = _refreshTokenRepository.CrearRefreshTokenAsync(nuevoRefreshTokenModel);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        _logger.LogInformation("Usuario con id = {usuarioId} renovo su resfrehToken correctamente",tokenEncontrado.UserId);

        return Result<AutenticacionResponseDto>.Success(new AutenticacionResponseDto
        {
            AccessToken = jwt,
            RefreshToken = refreshTokenCreado.Token
        });
    }
    public async Task<Result> Logout(RefreshTokenRenovarDto token)
    {
        var tokenEncontrado = await _refreshTokenRepository.ObtenerRefreshTokenPorTokenAsync(token.Token);
        var tokenTail = token.Token?.Length > 8
            ? token.Token[^8..]
            : token.Token;

        if (tokenEncontrado == null)
        {
            _logger.LogWarning("Refresh token no existe. TokenTail={TokenTail}", tokenTail);
            return Result.Failure("Su token no puede ser null");
        }

        if (tokenEncontrado.RevokedAt != null)
        {
            _logger.LogWarning("Usuario con id {usuarioId} envio un token revocado", tokenEncontrado.UserId);
            return Result.Failure("Su token fue revocado");
        }

        if (tokenEncontrado.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Usuario con id {usuarioId} envio un token expirado", tokenEncontrado.UserId);
            return Result.Failure("Su token ya expiro");
        }

        if (tokenEncontrado.IsUsed)
        {
            _logger.LogWarning("Usuario con id {usuarioId} envio un token usado", tokenEncontrado.UserId);
            return Result.Failure("Su token ya fue usado");
        }

        if (tokenEncontrado.Usuario == null)
        {
            _logger.LogWarning("No existe usuario asignado al token {tokenTail} ", tokenTail);
            return Result.Failure("Su usuario debe de existir");
        }

        tokenEncontrado.IsUsed = true;
        tokenEncontrado.RevokedAt = DateTime.UtcNow;

        await _unidadDeTrabajo.GuardarCambiosAsync();
        _logger.LogInformation("Usuario con id {usuarioId} se deslogeo",tokenEncontrado.UserId);

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
