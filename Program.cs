using imprimir.Models;
using imprimir.Queue;
using imprimir.Services;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("PrintApiCors", policy =>
    {
        if (allowedOrigins.Length == 0 || allowedOrigins.Contains("*"))
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            return;
        }

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<PrintService>();
builder.Services.AddScoped<IQueueService, QueueService>();
builder.Services.AddControllers();

// Queues
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

// Workers
builder.Services.AddHostedService<QueueHostedService>();

var app = builder.Build();

app.UseCors("PrintApiCors");

app.MapGet("/", () => Results.Ok(new { status = "ok", service = "print-api" }));
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "print-api" }));

//app.MapGet("/imprimir", GetPrintTicketStructure);
//app.MapGet("/api/print/ticket", GetPrintTicketStructure);

//app.MapPost("/imprimir", Print);
//app.MapPost("/api/print/ticket", Print);

app.MapControllers();

app.Run();

/*static IResult GetPrintTicketStructure()
{
    return Results.Ok(new PrintTicketRequestV2
    {
        PrinterName = null,
        Content = null,
        EventName = "Nombre del evento",
        PersonName = "Nombre de la persona",
        EventDate = "2026-06-15",
        EventTime = "8:00 PM",
        Venue = "Lugar del evento",
        OrderCode = "EVT-000123",
        TicketType = "General",
        Seats = ["Fila A - Silla 12"],
        QrContent = "EVT-000123|Nombre de la persona|Nombre del evento|Fila A - Silla 12"
    });
}*/

/*static async Task<IResult> Print(PrintTicketRequestV2? request, PrintService printer)
{
    if (request is null)
    {
        return Results.BadRequest(new PrintResponse
        {
            Success = false,
            Message = "Debe enviar un cuerpo JSON con content o los datos del evento."
        });
    }

    if (string.IsNullOrWhiteSpace(request.Content) && string.IsNullOrWhiteSpace(request.EventName))
    {
        return Results.BadRequest(new PrintResponse
        {
            Success = false,
            Message = "Debe enviar content o los datos del evento."
        });
    }

    var response = await printer.PrintAsync(request);
    return response.Success ? Results.Ok(response) : Results.BadRequest(response);
}*/
