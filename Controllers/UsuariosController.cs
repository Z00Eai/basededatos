using System.Security.Claims;
using GestorTareas.Data;
using GestorTareas.Helpers;
using GestorTareas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorTareas.Controllers;

[Authorize(Roles = "Administrador")]
public class UsuariosController : Controller
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Usuarios
    public async Task<IActionResult> Index()
    {
        var usuarios = await _context.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .ToListAsync();

        return View(usuarios);
    }

    // POST: Usuarios/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(string username, string nombreCompleto, string password, string rol)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(nombreCompleto))
        {
            TempData["Error"] = "Todos los campos son obligatorios para crear un usuario.";
            return RedirectToAction(nameof(Index));
        }

        username = username.Trim();
        var existe = await _context.Usuarios.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        if (existe)
        {
            TempData["Error"] = $"El usuario '{username}' ya existe.";
            return RedirectToAction(nameof(Index));
        }

        var nuevoUsuario = new Usuario
        {
            Username = username,
            NombreCompleto = nombreCompleto.Trim(),
            PasswordHash = PasswordHelper.HashPassword(password),
            Rol = rol == "Administrador" ? "Administrador" : "Usuario",
            FechaRegistro = DateTime.Now
        };

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = $"Usuario '{nuevoUsuario.Username}' creado con rol '{nuevoUsuario.Rol}'.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Usuarios/CambiarRol/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarRol(int id)
    {
        var currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(currentUserIdStr, out int currentUserId) && currentUserId == id)
        {
            TempData["Error"] = "No puedes cambiar tu propio rol de administrador.";
            return RedirectToAction(nameof(Index));
        }

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            TempData["Error"] = "Usuario no encontrado.";
            return RedirectToAction(nameof(Index));
        }

        usuario.Rol = usuario.Rol == "Administrador" ? "Usuario" : "Administrador";
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = $"Rol de '{usuario.Username}' actualizado a '{usuario.Rol}'.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Usuarios/Eliminar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(currentUserIdStr, out int currentUserId) && currentUserId == id)
        {
            TempData["Error"] = "No puedes eliminar tu propia cuenta mientras estás en sesión.";
            return RedirectToAction(nameof(Index));
        }

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = $"Usuario '{usuario.Username}' eliminado correctamente.";
        }

        return RedirectToAction(nameof(Index));
    }
}
