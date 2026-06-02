namespace imprimir.Models;

using System.ComponentModel.DataAnnotations;

public class PrintTicketRequest
{
    public string PrinterName { get; set; } = "";

    [Required(ErrorMessage = "El cliente es obligatorio.")]
    [MinLength(1, ErrorMessage = "El cliente es obligatorio.")]
    public string Customer { get; set; } = "";

    [Required(ErrorMessage = "El producto es obligatorio.")]
    [MinLength(1, ErrorMessage = "El producto es obligatorio.")]
    public string Product { get; set; } = "";

    [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0.")]
    public decimal Total { get; set; }
}
