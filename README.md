# Proyecto Backend de Facturación

Este es el backend para una aplicación de facturación, desarrollado con ASP.NET Core. Proporciona una API RESTful para gestionar clientes, productos y la creación de facturas.

## Descripción General

El proyecto consiste en una API RESTful que permite realizar operaciones CRUD (Crear, Leer, Actualizar, Borrar) sobre las siguientes entidades:

*   **Clientes**: Información de los compradores.
*   **Productos**: Catálogo de productos con información de stock y precios.
*   **Facturas**: Documentos de venta que asocian un cliente con una serie de productos.

La lógica de negocio más importante, la creación de una factura, se maneja de forma transaccional para garantizar la integridad de los datos. Esto incluye la validación del stock del producto, el cálculo de totales en el servidor y la actualización atómica del stock.

## Características Principales

*   **Framework**: Construido sobre ASP.NET Core.
*   **Acceso a Datos**: Utiliza Entity Framework Core en un enfoque "Code-First" para la gestión de la base de datos.
*   **Base de Datos**: Configurado por defecto para usar **SQL Server LocalDB**.
*   **Arquitectura**:
    *   Sigue el patrón de diseño Repositorio (implícito en el `DbContext`).
    *   Utiliza **DTOs (Data Transfer Objects)** para separar los modelos de la base de datos de los contratos de la API.
    *   Lógica de negocio encapsulada en los controladores.
*   **Documentación de API**: Integra **Scalar** para proporcionar una interfaz de usuario interactiva y documentación automática de los endpoints en el entorno de desarrollo.
*   **CORS**: Incluye una política de Cross-Origin Resource Sharing (CORS) preconfigurada para permitir solicitudes desde `http://localhost:4200`, facilitando la integración con un frontend (como Angular, React, etc.).

## Estructura del Proyecto

El proyecto está organizado en las siguientes carpetas principales dentro del directorio `Backend/`:

*   `Backend/Controllers`: Contiene los controladores de la API, que definen los endpoints RESTful para cada entidad (Clientes, Productos, Facturas).
*   `Backend/Models`: Contiene las clases C# que representan las entidades de la base de datos (ej. `Cliente.cs`, `Producto.cs`, `Factura.cs`).
*   `Backend/DTOs`: Contiene los Data Transfer Objects, que son modelos simplificados para la entrada y salida de datos en la API (ej. `FacturaCreateDto.cs`).
*   `Backend/Contex`: Contiene la clase `AppDBContex`, que es el DbContext de Entity Framework Core que gestiona la sesión con la base de datos.
*   `Backend/Migrations`: Almacena los archivos de migración generados por Entity Framework Core para crear y actualizar el esquema de la base de datos.
*   `Backend/appsettings.json`: Archivo de configuración principal, donde se define la cadena de conexión a la base de datos.

## Requisitos Previos

Para poder compilar y ejecutar este proyecto, necesitarás tener instalado lo siguiente:

*   **.NET SDK**: La versión del framework especificada en el archivo `.csproj` es `net10.0`. Esto parece ser una versión no estándar o un error tipográfico. Se recomienda usar una versión LTS reciente como **.NET 8.0**.
*   **Servidor de Base de Datos**: Una instancia de **SQL Server** (se puede usar Express, Developer o LocalDB). El proyecto está preconfigurado para buscar una instancia de `(localdb)\MSSQLLocalDB`.
*   **CLI de .NET**: Se instala automáticamente con el .NET SDK.
*   **IDE (Opcional)**: Visual Studio 2022 o Visual Studio Code.

## Guía de Instalación y Ejecución

Sigue estos pasos para poner en marcha el proyecto:

**1. Clonar el Repositorio**
```bash
git clone <URL_DEL_REPOSITORIO>
cd <CARPETA_DEL_PROYECTO>/Backend
```

**2. Configurar la Cadena de Conexión**

El proyecto está configurado para conectarse a una base de datos LocalDB. La cadena de conexión se encuentra en el archivo `Backend/appsettings.json`:

```json
"ConnectionStrings": {
  "Connection": "Server=(localdb)\\MSSQLLocalDB;Database=InvoiceAPI;Trusted_Connection=True;"
}
```

Si tu instancia de SQL Server es diferente, modifica esta cadena de conexión según corresponda.

**3. Aplicar las Migraciones de la Base de Datos**

Este comando creará la base de datos `InvoiceAPI` (si no existe) y todas las tablas necesarias.

Abre una terminal en el directorio `Backend/` y ejecuta:
```bash
dotnet ef database update
```

**4. Ejecutar la Aplicación**

Una vez que la base de datos esté configurada, puedes iniciar la API.

```bash
dotnet run
```

La API se ejecutará y escuchará en los puertos definidos en `Backend/Properties/launchSettings.json` (generalmente `5000` para HTTP y `5001` para HTTPS).

**5. Acceder a la Documentación de la API**

Una vez que la aplicación se esté ejecutando en el entorno de desarrollo, puedes acceder a la interfaz de Scalar para ver y probar los endpoints de la API. Abre tu navegador y ve a la URL base de tu aplicación (por ejemplo, `http://localhost:5000`).

```
http://localhost:5000/scalar
```