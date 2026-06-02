using System.Diagnostics;
using imprimir.Models;

namespace imprimir.Services;

public class LinuxPrintService : IServiceTicket
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LinuxPrintService> _logger;

    public LinuxPrintService(IConfiguration configuration, ILogger<LinuxPrintService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<PrintResponse> PrintTicketAsync(PrintTicketRequest request)
    {
        var printerName = string.IsNullOrWhiteSpace(request.PrinterName)
            ? _configuration["Printing:DefaultPrinterName"]
            : request.PrinterName;

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
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        if (!string.IsNullOrWhiteSpace(printerName))
        {
            processInfo.ArgumentList.Add("-d");
            processInfo.ArgumentList.Add(printerName);
        }

        processInfo.ArgumentList.Add(filePath);

        try
        {
            using var process = Process.Start(processInfo);

            if (process == null)
            {
                return new PrintResponse
                {
                    Success = false,
                    Message = "No se pudo iniciar el comando de impresion.",
                    PrinterName = printerName
                };
            }

            var standardError = await process.StandardError.ReadToEndAsync();
            var standardOutput = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                _logger.LogError("No se pudo imprimir el ticket. lp exit code: {ExitCode}. Error: {Error}",
                    process.ExitCode,
                    standardError);

                return new PrintResponse
                {
                    Success = false,
                    Message = "No se pudo imprimir el ticket.",
                    PrinterName = printerName,
                    Error = string.IsNullOrWhiteSpace(standardError) ? standardOutput : standardError
                };
            }

            return new PrintResponse
            {
                Success = true,
                Message = "Ticket enviado a la impresora.",
                PrinterName = printerName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo ejecutar el comando de impresion.");

            return new PrintResponse
            {
                Success = false,
                Message = "No se pudo ejecutar el comando de impresion.",
                PrinterName = printerName,
                Error = ex.Message
            };
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    public async Task<IReadOnlyList<PrinterInfo>> GetPrintersAsync()
    {
        var defaultPrinter = _configuration["Printing:DefaultPrinterName"];
        var processInfo = new ProcessStartInfo
        {
            FileName = "lpstat",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        processInfo.ArgumentList.Add("-a");

        try
        {
            using var process = Process.Start(processInfo);

            if (process == null)
                return [];

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                _logger.LogError("No se pudieron consultar las impresoras. lpstat exit code: {ExitCode}. Error: {Error}",
                    process.ExitCode,
                    error);
                return [];
            }

            return output
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => new PrinterInfo
                {
                    Name = name!,
                    IsDefault = string.Equals(name, defaultPrinter, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudieron consultar las impresoras.");
            return [];
        }
    }
}
