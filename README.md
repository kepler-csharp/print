# API de impresion

API ASP.NET para enviar tickets a una impresora termica en Ubuntu usando CUPS (`lp`).

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

## Endpoints

Consultar estado:

```http
GET /api/print/health
```

Consultar impresoras:

```http
GET /api/print/printers
```

Imprimir ticket:

```http
POST /api/print/ticket
Content-Type: application/json

{
  "printerName": "XP-58",
  "customer": "Cliente de prueba",
  "product": "Producto de prueba",
  "total": 12000
}
```

`printerName` es opcional. Si no se envia, usa `Printing:DefaultPrinterName` de `appsettings.json`.
