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

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "ok",
            service = "print-api"
        });
    }

    [HttpGet("printers")]
    public async Task<IActionResult> GetPrinters()
    {
        var printers = await _serviceTicket.GetPrintersAsync();
        return Ok(new
        {
            success = true,
            printers
        });
    }

    [HttpPost("ticket")]
    public async Task<IActionResult> PrintTicket([FromBody] PrintTicketRequest request)
    {
        if (request is null)
        {
            return BadRequest(new PrintResponse
            {
                Success = false,
                Message = "Debe enviar los datos del ticket."
            });
        }

        var result = await _serviceTicket.PrintTicketAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
