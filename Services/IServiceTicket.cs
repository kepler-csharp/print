using imprimir.Models;

namespace imprimir.Services;

public interface IServiceTicket
{
    Task<PrintResponse> PrintTicketAsync(PrintTicketRequest request);
    Task<IReadOnlyList<PrinterInfo>> GetPrintersAsync();
}
