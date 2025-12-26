using Microsoft.IdentityModel.Tokens;
using Mini_E_Commerce_API.DALs.AutenticacionRepositoryCarpeta;
using Mini_E_Commerce_API.DTOs.AutenticacionDtoCarpeta;
using Mini_E_Commerce_API.Models;
using Mini_E_Commerce_API.Models.Enums;
using Mini_E_Commerce_API.Services.AutenticacionServiceCarpeta;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AutenticacionService : IAutenticacionService
{
    private readonly IAutenticacionRepository _autenticacionRepository;
    private readonly IConfiguration _configuration;

    public AutenticacionService(
        IAutenticacionRepository autenticacionRepository,
        IConfiguration configuration)
    {
        _autenticacionRepository = autenticacionRepository;
        _configuration = configuration;
    }

    public async Task<Result<AutenticacionResponseDto>> RegistrarAsync(RegistroRequestDto dto)
    {
        var usuarioExistente = await _autenticacionRepository.ObtenerPorEmailAsync(dto.Email);

        if (usuarioExistente is not null)
            return Result<AutenticacionResponseDto>.Failure("El email ya está registrado");

        var usuario = new Usuario
        {
            Email = dto.Email,
            Name = dto.Nombre,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Rol = RolUsuario.User
        };

        await _autenticacionRepository.CrearAsync(usuario);

        var token = GenerarJwt(usuario);

        return Result<AutenticacionResponseDto>.Success(new AutenticacionResponseDto
        {
            AccessToken = token
        });
    }

    public async Task<Result<AutenticacionResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        var usuario = await _autenticacionRepository.ObtenerPorEmailAsync(dto.Email);

        if (usuario is null || !usuario.IsActive)
            return Result<AutenticacionResponseDto>.Failure("Credenciales inválidas");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            return Result<AutenticacionResponseDto>.Failure("Credenciales inválidas");

        var token = GenerarJwt(usuario);

        return Result<AutenticacionResponseDto>.Success(new AutenticacionResponseDto
        {
            AccessToken = token
        });
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
