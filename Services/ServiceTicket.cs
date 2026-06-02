using System.Diagnostics;
using System.Text;
using imprimir.Models;

namespace imprimir.Services;

public class PrintService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PrintService> _logger;

    public PrintService(IConfiguration configuration, ILogger<PrintService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    
    public async Task<PrintResponse> PrintAsync(PrintedTicket request)
    {
        var printerName = _configuration["Printing:DefaultPrinterName"];

        var filePath = Path.Combine(Path.GetTempPath(), $"ticket-{Guid.NewGuid():N}.bin");
        await File.WriteAllBytesAsync(filePath, BuildTicketBytes(request));

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

        processInfo.ArgumentList.Add("-o");
        processInfo.ArgumentList.Add("raw");
        processInfo.ArgumentList.Add(filePath);

        try
        {
            using var process = Process.Start(processInfo);

            if (process == null)
            {
                return new PrintResponse
                {
                    Success = false,
                    Message = "No se pudo iniciar el comando lp.",
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
                    Message = "No se pudo imprimir.",
                    PrinterName = printerName,
                    Error = string.IsNullOrWhiteSpace(standardError) ? standardOutput : standardError
                };
            }

            return new PrintResponse
            {
                Success = true,
                Message = "Enviado a la impresora.",
                PrinterName = printerName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo ejecutar el comando de impresion.");

            return new PrintResponse
            {
                Success = false,
                Message = "No se pudo ejecutar lp.",
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

    private static string Normalize(string content)
    {
        var normalizedContent = content.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal);

        return normalizedContent.EndsWith('\n')
            ? normalizedContent
            : normalizedContent + Environment.NewLine;
    }

    private static byte[] BuildTicketBytes(PrintedTicket request)
    {
        var bytes = new List<byte>();
        bytes.AddRange([0x1B, 0x40]);
        bytes.AddRange(Encoding.Latin1.GetBytes(Normalize(BuildTicketText(request))));

        if (!string.IsNullOrWhiteSpace(request.QrCode))
        {
            bytes.AddRange(Encoding.Latin1.GetBytes("\n        ESCANEA TU QR\n\n"));
            bytes.AddRange(BuildQrBytes(request.QrCode.Trim()));
            bytes.AddRange(Encoding.Latin1.GetBytes("\n\n"));
        }

        bytes.AddRange(Encoding.Latin1.GetBytes("\n\n"));
        return bytes.ToArray();
    }

    private static string BuildTicketText(PrintedTicket request)
    {
        return string.Join('\n',
            Center("FIRMEZA"),
            Line(),
            Center(request.EventName),
            "",
            Label("Correo de Cliente", request.CustomerEmail),
            Label("Fecha", request.ShowtimeDay),
            Label("Hora", request.ShowtimeHour),
            Label("Asientos", request.SeatLabel),
            Label("Codigo", request.QrCode),
            Line(),
            Center("Presenta este ticket"),
            Center("en el ingreso del evento"));
    }

    private static string Label(string label, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return $"{label}: -";

        const int labelWidth = 9;
        var prefix = $"{label}:".PadRight(labelWidth);
        return Wrap($"{prefix}{value.Trim()}");
    }

    private static string Wrap(string value)
    {
        const int width = 32;
        if (value.Length <= width)
            return value;

        var lines = new List<string>();
        var current = "";

        foreach (var word in value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            var candidate = current.Length == 0 ? word : $"{current} {word}";
            if (candidate.Length <= width)
            {
                current = candidate;
                continue;
            }

            if (current.Length > 0)
                lines.Add(current);

            current = word;
        }

        if (current.Length > 0)
            lines.Add(current);

        return string.Join('\n', lines);
    }

    private static string Center(string? value)
    {
        const int width = 32;
        value = string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();

        if (value.Length >= width)
            return Wrap(value);

        return value.PadLeft((width + value.Length) / 2).PadRight(width);
    }

    private static string Line() => "--------------------------------";

    private static byte[] BuildQrBytes(string qrContent)
    {
        var data = Encoding.UTF8.GetBytes(qrContent);
        var length = data.Length + 3;
        var pL = (byte)(length % 256);
        var pH = (byte)(length / 256);
        var bytes = new List<byte>();

        bytes.AddRange([0x1B, 0x61, 0x01]);
        bytes.AddRange([0x1D, 0x28, 0x6B, 0x04, 0x00, 0x31, 0x41, 0x32, 0x00]);
        bytes.AddRange([0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x43, 0x06]);
        bytes.AddRange([0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x45, 0x31]);
        bytes.AddRange([0x1D, 0x28, 0x6B, pL, pH, 0x31, 0x50, 0x30]);
        bytes.AddRange(data);
        bytes.AddRange([0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x51, 0x30]);
        bytes.AddRange([0x1B, 0x61, 0x00]);

        return bytes.ToArray();
    }
}
