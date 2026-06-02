# API simple de impresion

API ASP.NET minima para enviar texto a una impresora termica en Ubuntu usando CUPS (`lp`).

## Requisitos en Ubuntu

Instala y habilita CUPS:

```bash
sudo apt update
sudo apt install cups
sudo systemctl enable --now cups
```

Verifica que la impresora este registrada con el nombre `XP-58`:

```bash
lpstat -a
lpstat -d
```

Si la impresora no aparece, agregala desde la configuracion de impresoras de Ubuntu o desde CUPS.

## Ejecutar la API

```bash
dotnet run
```

La API queda publicada en:

```text
http://0.0.0.0:5249
```

Desde otro equipo de la misma red usa la IP del equipo Ubuntu:

```text
http://IP_DEL_UBUNTU:5249
```

## Uso

Consultar estado:

```http
GET /health
```

Consultar la estructura editable del ticket:

```http
GET /imprimir
```

Tambien queda disponible:

```http
GET /api/print/ticket
```

Imprimir un ticket de evento:

```http
POST /imprimir
Content-Type: application/json

{
  "eventName": "Concierto Firmeza Live",
  "personName": "Duvan Ramirez",
  "eventDate": "2026-06-15",
  "eventTime": "8:00 PM",
  "venue": "Teatro Principal",
  "ticketType": "VIP",
  "seats": [
    "Fila A - Silla 12",
    "Fila A - Silla 13"
  ],
  "orderCode": "EVT-000123",
  "qrContent": "EVT-000123|Duvan Ramirez|Concierto Firmeza Live|Fila A 12-13"
}
```

La API arma automaticamente el ticket bonito para impresora de 58 mm. `qrContent` es opcional; si llega, se imprime un QR al final.

Tambien puedes imprimir texto libre enviando `content`:

```json
{
  "content": "FIRMEZA\n--------------------------------\nTicket manual\n--------------------------------\nGracias\n",
  "qrContent": "https://example.com/ticket/EVT-000123"
}
```

`printerName` es opcional. Si no se envia, usa `Printing:DefaultPrinterName` de `appsettings.json`.

Tambien queda disponible `POST /api/print/ticket` por compatibilidad.

Respuesta correcta:

```json
{
  "success": true,
  "message": "Enviado a la impresora.",
  "printerName": "XP-58",
  "error": null
}
```

Respuesta con error:

```json
{
  "success": false,
  "message": "No se pudo imprimir.",
  "printerName": "XP-58",
  "error": "detalle del error"
}
```
