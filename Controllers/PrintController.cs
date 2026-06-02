using imprimir.DTOs;
using imprimir.Services;
using imprimir.Models;

namespace imprimir.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PrintController : ControllerBase
{
    private readonly IQueueService _queueService;
    
    public PrintController(IQueueService queueService)
    {
        _queueService = queueService;
    }

    [HttpPost]
    public async Task<IActionResult> Print([FromBody] PrintRequest request)
    {
        // Validating request
        /*if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        } Don't needed by [ApiController] */
        
        // Adding ticket by ticket inside the service
        foreach (var ticket in request.Tickets)
        {
            DateTime showtimeDate = DateTime.Parse(ticket.ShowtimeStart);
            
            await _queueService.AddQueuePrint(new PrintedTicket
            {
                TicketId = ticket.TicketId,
                QrCode = ticket.QrCode,
                SeatLabel = ticket.SeatLabel,
                EventName = ticket.EventName,
                ShowtimeDay = showtimeDate.ToString("yyyy-MM-dd"),
                ShowtimeHour = showtimeDate.ToString("HH:mm"),
                CustomerEmail = request.CustomerEmail,
            });
        }

        return Ok();
    }
}