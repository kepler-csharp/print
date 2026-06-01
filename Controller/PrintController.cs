using imprimir.Models;
using imprimir.Models;
using imprimir.Services;    
using Microsoft.AspNetCore.Mvc;

namespace imprimir.Controllers;

[ApiController]
[Route("api/print")]
public class PrintController : ControllerBase
{
    private readonly IServiceTicket _serviceTicket;

    public PrintController(IServiceTicket serviceTicket)
    {
        _serviceTicket = serviceTicket;
    }

    [HttpPost("ticket")]
    public async Task<IActionResult> PrintTicket([FromBody] PrintTicketRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PrinterName))
            return BadRequest("Debe enviar el nombre de la impresora.");

        var result = await _serviceTicket.PrintTicketAsync(request);

        if (!result)
            return BadRequest("No se pudo imprimir el ticket.");

        return Ok("Ticket enviado a la impresora.");
    }
}