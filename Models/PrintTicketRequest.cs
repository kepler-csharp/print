namespace imprimir.Models;

public class PrintTicketRequest
{
    public string PrinterName { get; set; } = "";
    public string Customer { get; set; } = "";
    public string Product { get; set; } = "";
    public decimal Total { get; set; }
}
