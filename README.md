# Cirkula API - Prueba Técnica Backend

API REST desarrollada en ASP.NET Core 8 para gestionar tiendas con cálculo de distancia y estado de apertura en tiempo real.

**Demo:** https://prueba-tecnica-cirkula-backend-production.up.railway.app/swagger

---

## Tecnologías

- **ASP.NET Core 8** Web API
- **Entity Framework Core** con SQLite
- **Cloudinary** para almacenamiento de imágenes
- **Railway** para deploy

---

## Requisitos previos

- .NET 8 SDK
- Cuenta en Cloudinary (gratuita)

---

## Configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/Gab16El/prueba-tecnica-cirkula-backend.git
cd prueba-tecnica-cirkula-backend
```

### 2. Configurar Cloudinary

Ve a [cloudinary.com](https://cloudinary.com), crea una cuenta gratuita y obtén tus credenciales desde el **Dashboard**.

Crea el archivo `appsettings.Development.json` en la raíz del proyecto:

```json
{
  "Cloudinary": {
    "CloudName": "tu-cloud-name",
    "ApiKey": "tu-api-key",
    "ApiSecret": "tu-api-secret"
  }
}
```

> Este archivo está en `.gitignore` para proteger las credenciales.

### 3. Correr migraciones

```bash
dotnet ef database update
```

### 4. Correr el proyecto

```bash
dotnet run
```

Abre el navegador en `http://localhost:5000/swagger`

---

## Endpoints

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/stores?latitude={lat}&longitude={lng}` | Lista tiendas con distancia e isOpen |
| GET | `/api/stores/{id}` | Detalle de una tienda |
| POST | `/api/stores` | Crear tienda (multipart/form-data) |
| PUT | `/api/stores/{id}` | Actualizar tienda (multipart/form-data) |
| DELETE | `/api/stores/{id}` | Eliminar tienda |

### Ejemplo de respuesta GET

```json
{
  "stores": [
    {
      "id": 1,
      "name": "Tienda A",
      "bannerUrl": "https://res.cloudinary.com/.../banner.jpg",
      "latitude": -12.0464,
      "longitude": -77.0428,
      "openTime": "09:00 AM",
      "closeTime": "07:00 PM",
      "distanceInKm": 1.5,
      "isOpen": true
    }
  ]
}
```

---

## Modelo de datos

```csharp
public class Store {
    public int Id { get; set; }
    public string Name { get; set; }
    public string BannerUrl { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string OpenTime { get; set; }  // formato: "09:00 AM"
    public string CloseTime { get; set; } // formato: "07:00 PM"
}
```

---

## Cálculo de distancia

Se usa la fórmula de Haversine para calcular la distancia en kilómetros entre las coordenadas del usuario y cada tienda.

---

## Deploy en Railway

Variables de entorno necesarias en Railway:

```
ASPNETCORE_URLS=http://0.0.0.0:8080
Cloudinary__CloudName=tu-cloud-name
Cloudinary__ApiKey=tu-api-key
Cloudinary__ApiSecret=tu-api-secret
```

---

## Repositorios relacionados

- Frontend móvil (React Native): [prueba-tecnica-cirkula-frontend](https://github.com/Gab16El/prueba-tecnica-cirkula-frontend)
- Panel web (React): [prueba-tecnica-cirkula-web](https://github.com/Gab16El/prueba-tecnica-cirkula-web) — [Demo](https://panel-cirkula-ngmm.netlify.app/)