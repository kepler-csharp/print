namespace imprimir.Models;

public class PrintTicketRequest
{
    public string? PrinterName { get; set; }
    public string? Content { get; set; }
    public string? EventName { get; set; }
    public string? PersonName { get; set; }
    public string? EventDate { get; set; }
    public string? EventTime { get; set; }
    public string? Venue { get; set; }
    public string? OrderCode { get; set; }
    public string? TicketType { get; set; }
    public string[] Seats { get; set; } = [];
    public string? QrContent { get; set; }
}
