using Mini_E_Commerce_API.DALs.ProductoRepositoryCarpeta;

namespace Mini_E_Commerce_API.Services.ProductoServiceCarpeta
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }
    }
}
