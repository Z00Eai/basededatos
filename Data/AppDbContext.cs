using Microsoft.EntityFrameworkCore;
using GestorTareas.Models;

namespace GestorTareas.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tarea> Tareas { get; set; } = null!;
    public DbSet<LibroRenta> Libros { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}
