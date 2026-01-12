using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.Common.Errors
{
    public static class DomainErrors
    {
        public static class User
        {
            public static Error NotFound(int usuarioId) =>
                new("User.NotFound", $"El usuario con id = {usuarioId} no existe.", ErrorType.NotFound);
        }

        public static class Product
        {
            public static Error NotFound(int id) =>
                new("Product.NotFound", $"El producto con id={id} no existe.", ErrorType.NotFound);


            public static readonly Error InvalidId =
                new("Product.InvalidId", "Su productId debe de ser mayor a 0", ErrorType.Validation);


            public static readonly Error InvalidStock =
                new("Product.InvalidStock", "El stock debe de ser mayor a 0", ErrorType.Validation);


            public static readonly Error InvalidPrice =
                new("Product.InvalidPrice", "El precio debe de ser mayor a 0", ErrorType.Validation);


            public static readonly Error InvalidAmount =
                new("Product.InvalidAmount", "La cantidad debe de ser mayor a 0", ErrorType.Validation);


            public static Error InssuficientStock(int productoId) =>
                new("Product.InssuficientStock", $"Su producto con id = {productoId} no tiene stock suficiente", ErrorType.Conflict);

            public static Error Inactive(int id) =>
                new("Product.Inactive", $"El producto con id={id} está inactivo.", ErrorType.Conflict);


            public static readonly Error Conflict =
                new("Product.Conflict", $"No puede existir dos productos con el mismo nombre en la misma categoría", ErrorType.Conflict);
        }

        public static class Category
        {
            public static Error NotFound(int id) =>
                new("Category.NotFound", $"La categoria con id={id} no existe.", ErrorType.NotFound);
            public static Error Inactive(int id) =>
              new("Category.Inactive", $"La categoria con id={id} está inactiva.", ErrorType.Conflict);

        }
    }
}
