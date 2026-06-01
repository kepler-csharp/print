using System.Diagnostics;
using imprimir.Models;


namespace imprimir.Services;

public class LinuxPrintService : IServiceTicket
{
    public async Task<bool> PrintTicketAsync(PrintTicketRequest request)
    {
        var ticket = $"""
                      FIRMEZA
                      -------------------------
                      Cliente: {request.Customer}
                      Producto: {request.Product}
                      Total: ${request.Total}
                      -------------------------
                      Gracias por su compra

                      """;

        var filePath = $"/tmp/ticket-{Guid.NewGuid()}.txt";

        await File.WriteAllTextAsync(filePath, ticket);

        var processInfo = new ProcessStartInfo
        {
            FileName = "lp",
            Arguments = $"-d \"{request.PrinterName}\" \"{filePath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(processInfo);

        if (process == null)
            return false;

        await process.WaitForExitAsync();

        File.Delete(filePath);

        return process.ExitCode == 0;
    }
}