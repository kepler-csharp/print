using imprimir.Models;
using imprimir.Queue;

namespace imprimir.Services;

public class QueueService : IQueueService
{
    private readonly IBackgroundTaskQueue _queue;
    private readonly PrintService _printService;
    
    public QueueService(IBackgroundTaskQueue queue, PrintService printService)
    {
        _queue = queue;
        _printService = printService;
    }

    public async Task AddQueuePrint(PrintedTicket ticket)
    {
        Console.WriteLine($"QUEUEING {ticket.TicketId}");
        
        await _queue.QueueAsync(async token =>
            {
                Console.WriteLine($"WORKER START {ticket.TicketId}");
                
                // Printing Service
                await _printService.PrintAsync(ticket);
                
                Console.WriteLine($"WORKER END {ticket.TicketId}");
            }
        );
    }
}