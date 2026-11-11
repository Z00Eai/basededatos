using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestorTareas.Data;
using GestorTareas.Models;

namespace GestorTareas.Controllers;

public class LibrosController : Controller
{
    private readonly AppDbContext _context;

    public LibrosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Libros
    public async Task<IActionResult> Index(string? genero, string? buscar)
    {
        // Sembrar datos iniciales si la biblioteca está vacía
        if (!await _context.Libros.AnyAsync())
        {
            _context.Libros.AddRange(
                new LibroRenta { Titulo = "Cien años de soledad", Autor = "Gabriel García Márquez", Genero = "Realismo Mágico", Rentado = false },
                new LibroRenta { Titulo = "1984", Autor = "George Orwell", Genero = "Ciencia Ficción", Rentado = false },
                new LibroRenta { Titulo = "El Principito", Autor = "Antoine de Saint-Exupéry", Genero = "Fábula", Rentado = true, RentadoPor = "Ana Pérez", FechaRenta = DateTime.Now.AddDays(-2), FechaDevolucion = DateTime.Now.AddDays(5) },
                new LibroRenta { Titulo = "Don Quijote de la Mancha", Autor = "Miguel de Cervantes", Genero = "Clásico", Rentado = false },
                new LibroRenta { Titulo = "El Alquimista", Autor = "Paulo Coelho", Genero = "Filosofía / Ficción", Rentado = false },
                new LibroRenta { Titulo = "Fahrenheit 451", Autor = "Ray Bradbury", Genero = "Ciencia Ficción", Rentado = false }
            );
            await _context.SaveChangesAsync();
        }

        var query = _context.Libros.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(genero) && genero != "Todos")
        {
            query = query.Where(l => l.Genero == genero);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            query = query.Where(l => l.Titulo.Contains(buscar) || l.Autor.Contains(buscar));
        }

        var libros = await query.OrderBy(l => l.Rentado).ThenBy(l => l.Titulo).ToListAsync();
        ViewBag.Generos = await _context.Libros.Select(l => l.Genero).Distinct().ToListAsync();
        ViewBag.GeneroActual = genero ?? "Todos";
        ViewBag.BuscarActual = buscar ?? "";

        return View(libros);
    }

    // POST: Libros/Rentar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rentar(int id, string rentadoPor)
    {
        var libro = await _context.Libros.FindAsync(id);
        if (libro != null && !libro.Rentado)
        {
            libro.Rentado = true;
            libro.RentadoPor = string.IsNullOrWhiteSpace(rentadoPor) ? "Lector Anónimo" : rentadoPor;
            libro.FechaRenta = DateTime.Now;
            libro.FechaDevolucion = DateTime.Now.AddDays(7);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = $"¡Has rentado '{libro.Titulo}' exitosamente! Devolución en 7 días.";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: Libros/Devolver/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Devolver(int id)
    {
        var libro = await _context.Libros.FindAsync(id);
        if (libro != null && libro.Rentado)
        {
            libro.Rentado = false;
            libro.RentadoPor = null;
            libro.FechaRenta = null;
            libro.FechaDevolucion = null;
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = $"El libro '{libro.Titulo}' ha sido devuelto a la biblioteca.";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: Libros/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear([Bind("Titulo,Autor,Genero")] LibroRenta libro)
    {
        if (ModelState.IsValid)
        {
            libro.Rentado = false;
            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = $"Libro '{libro.Titulo}' añadido al catálogo.";
        }
        return RedirectToAction(nameof(Index));
    }
}
