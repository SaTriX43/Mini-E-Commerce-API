using Microsoft.EntityFrameworkCore;
using Mini_E_Commerce_API.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Carrito> Carritos => Set<Carrito>();
    public DbSet<CarritoItem> CarritoItems => Set<CarritoItem>();
    public DbSet<Orden> Ordenes => Set<Orden>();
    public DbSet<OrdenDetalle> OrdenDetalles => Set<OrdenDetalle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.Usuario)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => rt.Token)
            .IsUnique();

        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<Carrito>()
            .HasOne(c => c.Usuario)
            .WithOne()
            .HasForeignKey<Carrito>(c => c.UserId);

        modelBuilder.Entity<CarritoItem>()
            .HasOne(ci => ci.Carrito)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId);

        modelBuilder.Entity<CarritoItem>()
            .HasOne(ci => ci.Producto)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId);

        modelBuilder.Entity<Orden>()
            .HasOne(o => o.Usuario)
            .WithMany()
            .HasForeignKey(o => o.UserId);

        modelBuilder.Entity<OrdenDetalle>()
            .HasOne(od => od.Orden)
            .WithMany(o => o.Detalles)
            .HasForeignKey(od => od.OrderId);

        modelBuilder.Entity<OrdenDetalle>()
            .HasOne(od => od.Producto)
            .WithMany()
            .HasForeignKey(od => od.ProductId);
    }
}
