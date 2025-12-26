using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.AutenticacionRepositoryCarpeta
{
    public interface IAutenticacionRepository
    {
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task CrearAsync(Usuario usuario);
    }

}
