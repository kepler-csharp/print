namespace imprimir.Models;

public class PrintedTicket
{
    public int TicketId { get; set; }
    public string QrCode { get; set; }
    public string SeatLabel { get; set; }
    public string EventName { get; set; }
    public string ShowtimeDay { get; set; }
    public string ShowtimeHour { get; set; }
    
    public string CustomerEmail { get; set; }
}