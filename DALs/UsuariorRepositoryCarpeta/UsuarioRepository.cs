using Microsoft.EntityFrameworkCore;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.UsuariorRepositoryCarpeta
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context) { 
            _context = context;
        }
        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int usuarioId)
        {
            var usuarioEncontrado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId);
            return usuarioEncontrado;
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario> CrearAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
    }
}
