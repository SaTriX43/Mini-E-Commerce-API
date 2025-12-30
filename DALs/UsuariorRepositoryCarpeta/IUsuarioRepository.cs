using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.UsuariorRepositoryCarpeta
{
    public interface IUsuarioRepository
    {
        public Task<Usuario?> ObtenerUsuarioPorIdAsync(int usuarioId);
    }
}
