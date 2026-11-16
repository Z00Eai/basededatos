using GestorTareas.Helpers;
using GestorTareas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorTareas.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Aplicar migraciones si hubiera alguna pendiente
        await context.Database.MigrateAsync();

        // Sembrar usuarios iniciales si no existen
        if (!await context.Usuarios.AnyAsync())
        {
            var admin = new Usuario
            {
                Username = "admin",
                NombreCompleto = "Administrador del Sistema",
                PasswordHash = PasswordHelper.HashPassword("Admin123!"),
                Rol = "Administrador",
                FechaRegistro = DateTime.Now
            };

            var user = new Usuario
            {
                Username = "usuario",
                NombreCompleto = "Usuario Estándar",
                PasswordHash = PasswordHelper.HashPassword("User123!"),
                Rol = "Usuario",
                FechaRegistro = DateTime.Now
            };

            context.Usuarios.AddRange(admin, user);
            await context.SaveChangesAsync();
        }
    }
}
