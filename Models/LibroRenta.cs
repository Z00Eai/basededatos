using System.ComponentModel.DataAnnotations;

namespace GestorTareas.Models;

public class LibroRenta
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(120)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El autor es obligatorio")]
    [StringLength(100)]
    public string Autor { get; set; } = string.Empty;

    [Required(ErrorMessage = "El género es obligatorio")]
    [StringLength(50)]
    public string Genero { get; set; } = "Novela";

    public bool Rentado { get; set; } = false;

    [StringLength(100)]
    public string? RentadoPor { get; set; }

    [DataType(DataType.Date)]
    public DateTime? FechaRenta { get; set; }

    [DataType(DataType.Date)]
    public DateTime? FechaDevolucion { get; set; }
}
