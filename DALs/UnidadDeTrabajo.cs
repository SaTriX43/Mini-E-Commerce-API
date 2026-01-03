namespace Mini_E_Commerce_API.DALs
{
    public class UnidadDeTrabajo : IUnidadDeTrabajo
    {
        private readonly ApplicationDbContext _context;

        public UnidadDeTrabajo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
