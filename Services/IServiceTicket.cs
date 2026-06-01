using imprimir.Models;

namespace imprimir.Services;

public interface IServiceTicket
{
    public Task<bool> PrintTicketAsync(PrintTicketRequest request);
}