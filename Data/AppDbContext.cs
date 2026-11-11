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
}
