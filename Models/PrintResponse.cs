namespace imprimir.Models;

public class PrintResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public string? PrinterName { get; set; }
    public string? Error { get; set; }
}
