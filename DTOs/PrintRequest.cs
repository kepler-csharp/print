namespace imprimir.DTOs;

using System.ComponentModel.DataAnnotations;

public class PrintRequest
{
    [Required]
    public string CustomerEmail { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<Ticket> Tickets { get; set; }
}

public class Ticket
{
    [Required]
    public int TicketId { get; set; }
    
    [Required]
    public string QrCode { get; set; }
    
    [Required]
    public string QrImageUrl { get; set; }
    
    [Required]
    public string SeatLabel { get; set; }
    
    [Required]
    public string EventName { get; set; }
    
    [Required]
    public string ShowtimeStart { get; set; }
    
    [Required]
    public bool IsUsed { get; set; }
    
    public string? UsedAt { get; set; }
}