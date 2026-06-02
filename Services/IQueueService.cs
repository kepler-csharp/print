using imprimir.Models;

namespace imprimir.Services;

public interface IQueueService
{
    Task AddQueuePrint(PrintedTicket ticket);
}