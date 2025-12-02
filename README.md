# Cliente (Frontend) - Tiendita POS Ultimate

Aplicación de escritorio nativa, ultra-ligera y multiplataforma. Utiliza tecnología web (Blazor Hybrid) dentro de una ventana nativa del sistema operativo, eliminando la necesidad de un navegador externo.

## Tecnologías Utilizadas

### Core
* **.NET 9.0**: Framework base de última generación.
* **C#**: Lenguaje principal para lógica de negocio y control.
* **Photino.Blazor**: Motor ligero para renderizar Blazor en escritorio (alternativa a MAUI/Electron).

### Interfaz de Usuario (UI/UX)
* **HTML5 & CSS3**: Diseño visual.
* **Glassmorphism**: Estilo visual moderno con transparencias y sombras.
* **Bootstrap 5**: Grid system y utilidades de diseño.
* **Bootstrap Icons**: Iconografía vectorial.
* **Chart.js**: Gráficas interactivas para el Dashboard.

### Comunicación & Datos
* **RestSharp**: Cliente HTTP para consumir la API REST.
* **SocketIOClient**: Cliente WebSocket para recibir alertas en tiempo real.
* **Newtonsoft.Json**: Serialización y manejo de modelos JSON.

### Reportes
* **QuestPDF**: Generación de reportes PDF profesionales mediante código fluido.
* **ClosedXML**: Generación de reportes en Excel (.xlsx) sin dependencia de Office.

---

## Instalación y Ejecución

### 1. Requisitos Previos
* Tener instalado el SDK de .NET 9.0.
* (Opcional) WebView2 Runtime instalado en Windows (viene por defecto en W10/11 actualizados).

### 2. Configuración de Conexión
Edita el archivo `appsettings.json` para apuntar a tu servidor backend:

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5000/api/", 
    "UseRemote": false
  },
  "RemoteSettings": {
    "BaseUrl": "[https://tu-servidor-en-la-nube.com/api/](https://tu-servidor-en-la-nube.com/api/)"
  }
}