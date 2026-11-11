vamos messi
# AorusMarket – Sistema de Gestión para Supermercados

Aplicación de escritorio para Windows que permite gestionar un supermercado con **varias sucursales**: punto de venta, stock por sucursal, catálogo de productos, clientes, usuarios con perfiles, reportes, auditoría y copias de seguridad de la base de datos.
POr Acuña Bruno y Acuña Alejo jeje

Desarrollada en **C# (Windows Forms, .NET Framework 4.8)** con base de datos **SQL Server**.

---

## Índice

1. [Funcionalidades](#funcionalidades)
2. [Perfiles y permisos](#perfiles-y-permisos)
3. [Tecnologías](#tecnologías)
4. [Arquitectura del proyecto](#arquitectura-del-proyecto)
5. [Base de datos](#base-de-datos)
6. [Guía de instalación](#guía-de-instalación)
7. [Primer inicio de sesión](#primer-inicio-de-sesión)
8. [Backups en Google Drive (opcional)](#backups-en-google-drive-opcional)
9. [Solución de problemas](#solución-de-problemas)

---

## Funcionalidades

- **Punto de venta (POS):** búsqueda por código de barras o nombre, carrito, cliente opcional ("Consumidor Final"), métodos de pago (Efectivo, Débito, Crédito, Transferencia) y cálculo de vuelto.
  - Las ventas de **más de $50.000** requieren la autorización (usuario y contraseña) de un Administrador o Gerente.
  - Al confirmar se descuenta el stock y se puede generar **ticket** y/o **factura** en PDF.
- **Stock por sucursal:** cantidad, precio y stock mínimo independientes en cada sucursal. Cada cambio queda registrado como movimiento de stock.
- **Productos y categorías:** ABM con validación de duplicados y de código de barras.
- **Clientes:** ABM con validación de DNI, CUIL/CUIT, email, teléfono y dirección.
- **Usuarios y sucursales:** gestión completa, con protección para no eliminar ni degradar al único Administrador.
- **Historial de ventas:** consulta por fechas y cajero, detalle de cada venta y **anulación** (devuelve el stock automáticamente).
- **Dashboard:** total vendido, cantidad de ventas, producto más vendido, ventas recientes y **alertas de stock mínimo**, filtrable por sucursal y rango de fechas.
- **Reportes:** recaudación por medio de pago, evolución de ventas, ventas por sucursal, por cajero y por categoría, con gráficos. Exportables a **PDF** y **Excel**.
- **Auditoría general:** registro de inicios de sesión (incluidos los fallidos), altas, bajas, modificaciones, ventas, anulaciones, ajustes de stock, backups y accesos denegados.
- **Backups y restauración:**
  - Backup manual o **automático diario** a una hora configurable, con verificación del archivo (`RESTORE VERIFYONLY`) y retención de las últimas *N* copias.
  - Destino: carpeta local, red/NAS o Google Drive.
  - **Restauración con doble autorización:** requiere las credenciales de un segundo usuario (Administrador o Gerente) y genera antes una copia de seguridad del estado actual.
- **Exportación a Excel** en historial, auditoría y stock.

## Perfiles y permisos

| Módulo | Administrador | Gerente | Cajero | Gestor de Stock |
|---|:-:|:-:|:-:|:-:|
| Punto de Venta | | | ✅ | |
| Clientes | ✅ | ✅ | ✅ | |
| Historial de ventas (anular: Admin y Gerente) | ✅ | ✅ | ✅ (solo ver) | |
| Stock / Productos / Categorías | ✅ | ✅ | | ✅ |
| Dashboard y Reportes | ✅ | ✅ | | |
| Backups y restauración | ✅ | ✅ | | |
| Usuarios y Sucursales | ✅ | | | |
| Auditoría general | ✅ | | | |

Los permisos se controlan en dos niveles: el **menú** muestra solo lo que corresponde a cada perfil, y la **capa de negocio** vuelve a verificar el perfil en cada operación (y registra los intentos denegados en la auditoría).

## Tecnologías

- C# / Windows Forms – .NET Framework 4.8
- SQL Server (ADO.NET con consultas parametrizadas y transacciones)
- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net) – hash de contraseñas
- [ReaLTaiizor](https://github.com/Taiizor/ReaLTaiizor) – controles de interfaz
- [iTextSharp 5](https://github.com/itext/itextsharp) – PDFs (ticket, factura, reportes)
- [ClosedXML](https://github.com/ClosedXML/ClosedXML) – exportación a Excel
- Google Drive API v3 – backups en la nube (opcional)

## Arquitectura del proyecto

Arquitectura en capas: cada capa solo habla con la siguiente.

```
Formularios  →  Negocio  →  AccesoDatos  →  SQL Server
     ↑             ↑             ↑
     └─────────── Entidades ─────┘        Utilidades (transversal)
```

```
Proyecto-aplicacion/
├── Formularios/   Interfaz (FrmLogin, MDIParent1, FrmPuntoVenta, FrmStock, FrmDashboard, ...)
├── Negocio/       Reglas de negocio, validaciones y permisos (VentaNegocio, UsuarioNegocio, BackupNegocio, ...)
├── AccesoDatos/   Consultas SQL y transacciones (VentaDatos, StockDatos, UsuarioDatos, Conexion, ...)
├── Entidades/     Clases de datos (Usuario, Producto, Venta, Cliente, ...)
├── Utilidades/    Sesión actual, logger, validaciones, estilos, generador de PDF/Excel, programador de backups
├── App.config     Cadena de conexión a la base de datos
└── sql.txt        Script de la base de datos (esquema + datos de ejemplo)
```

Detalles destacados:

- **Transacciones:** confirmar y anular una venta, y guardar usuarios con su dirección, se ejecutan en una transacción: o se guarda todo o no se guarda nada.
- **Control de stock concurrente:** el descuento usa `UPDATE ... WHERE cantidad >= @cant`, por lo que nunca se vende más stock del disponible.
- **Bajas lógicas:** usuarios, productos, categorías, clientes, sucursales y stock se marcan como inactivos para conservar el historial.
- **Logs de errores:** se guardan en `%LocalAppData%\AorusMarket\Logs` (un archivo por día, se conservan 30 días).

## Base de datos

Base: **SuperMercadoDB**. Tablas principales:

| Tabla | Contenido |
|---|---|
| `usuario`, `perfil`, `sucursal` | Usuarios, perfiles y sucursales |
| `producto`, `categoria` | Catálogo |
| `stock_sucursal`, `movimiento_stock` | Stock y precio por sucursal; historial de movimientos |
| `venta`, `detalleVenta` | Ventas (estado 1 = confirmada, 2 = anulada) y sus ítems |
| `cliente`, `direccion` | Clientes y direcciones |
| `auditoria_acciones` | Auditoría de acciones |
| `configuracion_backup`, `backup_registro` | Configuración y registro de backups/restauraciones |
| `historial_backups` y procedimientos `SP_*` | Esquema anterior de backups con claves cruzadas (la aplicación actual no los utiliza) |

---

## Guía de instalación

### Requisitos

| Componente | Detalle |
|---|---|
| Sistema operativo | Windows 10 o superior |
| .NET Framework | 4.8 (ya incluido en Windows 10 actualizado y Windows 11) |
| SQL Server | **SQL Server Express** con una instancia llamada `SQLEXPRESS` ([descarga](https://www.microsoft.com/sql-server/sql-server-downloads)) |
| Herramienta de BD | SQL Server Management Studio – SSMS ([descarga](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms)) |
| Cuenta de Google | Solo si querés backups en Google Drive (opcional) |
| Para compilar el código | Visual Studio con la carga de trabajo **"Desarrollo de escritorio con .NET"** |

### Paso 1 – Instalar SQL Server Express y SSMS

1. Instalá **SQL Server Express** con la instalación **Básica**. Por defecto crea la instancia `SQLEXPRESS` con autenticación de Windows, que es lo que usa la aplicación.
2. Instalá **SSMS**.

### Paso 2 – Crear la base de datos

1. Abrí **SSMS** y conectate a `.\SQLEXPRESS` (autenticación de Windows).
2. Menú **Archivo → Abrir → Archivo…** y elegí `sql.txt` de este repositorio.
3. Presioná **F5** (Ejecutar). Se crean la base `SuperMercadoDB`, sus tablas y los datos de ejemplo.
4. Verificá en el Explorador de objetos que aparezca `SuperMercadoDB`.

> **Importante – versión de SQL Server.** El script está generado con **SQL Server 2025** (carpeta `MSSQL17`, nivel de compatibilidad `170`). Si usás otra versión, antes de ejecutarlo hacé estos ajustes al inicio del script:
>
> | Versión | Ruta `MSSQL##` | `COMPATIBILITY_LEVEL` | Líneas a borrar |
> |---|---|---|---|
> | SQL Server 2025 | `MSSQL17` | 170 | ninguna |
> | SQL Server 2022 | `MSSQL16` | 160 | `SET OPTIMIZED_LOCKING = OFF` |
> | SQL Server 2019 | `MSSQL15` | 150 | `SET OPTIMIZED_LOCKING = OFF` y `LEDGER = OFF` (dentro del `CREATE DATABASE`) |
>
> Además, en el `CREATE DATABASE` las rutas `FILENAME` apuntan a `C:\Program Files\Microsoft SQL Server\MSSQL17.SQLEXPRESS\MSSQL\DATA\`. Si tu instalación está en otra carpeta, corregí esas dos rutas.

### Paso 3 – Preparar la carpeta de backups

La aplicación guarda los backups por defecto en `C:\AorusMarket\Backups\`. El que escribe esos archivos es el **servicio de SQL Server**, por lo que necesita permiso sobre la carpeta. Abrí **PowerShell como administrador** y ejecutá:

```powershell
New-Item -ItemType Directory -Force -Path 'C:\AorusMarket\Backups'
icacls 'C:\AorusMarket\Backups' /grant 'NT SERVICE\MSSQL$SQLEXPRESS:(OI)(CI)M'
```

### Paso 4 – Obtener y ejecutar la aplicación

**Opción A – Compilar desde el código (Visual Studio)**

1. Cloná el repositorio:
   ```bash
   git clone https://github.com/ThKo55/Proyecto-aplicacion.git
   ```
2. Abrí `AorusMarket.slnx` con Visual Studio (si tu versión no abre `.slnx`, abrí directamente `AorusMarket.csproj`).
3. Visual Studio restaura los paquetes NuGet automáticamente. Si no lo hace: clic derecho sobre la solución → **Restaurar paquetes NuGet**.
4. Seleccioná la configuración **Release** y compilá con **Ctrl + Shift + B**.
5. Ejecutá con **F5**, o abrí `bin\Release\AorusMarket.exe`.

**Opción B – Usar la versión compilada**

Copiá la carpeta `bin\Release` completa (el `.exe` necesita las `.dll` y el `.config` que están al lado) a la PC donde se va a usar y ejecutá `AorusMarket.exe`.

### Paso 5 – Verificar la conexión (solo si cambia algo)

La conexión está en `App.config` (en el ejecutable se llama `AorusMarket.exe.config`):

```xml
<add name="CadenaSQL"
     connectionString="Server=.\SQLEXPRESS;Database=SuperMercadoDB;Integrated Security=True;" />
```

Si tu instancia se llama distinto, o SQL Server está en otra PC, modificá el valor de `Server`. Ejemplos: `Server=localhost;...`, `Server=NOMBRE-PC\OTRAINSTANCIA;...`.

### Paso 6 – Habilitar los backups a Google Drive (opcional)

Este paso solo hace falta si querés que los backups también se suban a Google Drive. Sin él, la aplicación funciona igual y guarda los backups en la carpeta local.

1. Descargá el archivo **`credentials.json`** que está en este repositorio: abrilo en GitHub y usá el botón **Download raw file** (o clonando el repo ya lo tenés).
2. Copialo dentro de **`bin\Debug`**, en la misma carpeta que `AorusMarket.exe`. Esa carpeta se crea después de compilar (Paso 4).
   - Si ejecutás la versión *Release*, copialo en `bin\Release`.
   - El nombre debe ser exactamente `credentials.json`.
3. Abrí la aplicación, entrá al módulo **Backups** y elegí el destino **Google Drive (Nube)**.
4. En el primer backup se abre el navegador: iniciá sesión con tu cuenta de Google y aceptá el permiso. La autorización queda guardada en la carpeta `DriveToken` (junto al `.exe`) y no se vuelve a pedir.

> Los backups se suben al Drive de la cuenta de Google con la que autorices. Si al iniciar sesión Google muestra "acceso bloqueado" o "la app no está verificada", la cuenta tiene que estar agregada como **usuario de prueba** en el proyecto de Google Cloud al que pertenece el `credentials.json`.

---

## Primer inicio de sesión

Usuarios de ejemplo incluidos en el script:

| Usuario (email) | Perfil | Contraseña |
|---|---|---|
| `admin` | Administrador | _(completar)_ |
| `ventas` | Cajero | _(completar)_ |
| `stock` | Gestor de Stock | _(completar)_ |

> Por seguridad, **cambiá las contraseñas** desde el módulo *Usuarios* apenas ingreses. Las contraseñas nuevas deben tener al menos 8 caracteres y combinar letras y números.

## Backups en Google Drive (opcional)

Los backups pueden subirse automáticamente a Google Drive. Para activarlo hay que descargar el `credentials.json` del repositorio y colocarlo junto al ejecutable: ver el [Paso 6 de la guía de instalación](#paso-6--habilitar-los-backups-a-google-drive-opcional).

Si no se configura, los backups se guardan igualmente en la carpeta local.

## Solución de problemas

| Problema | Causa probable y solución |
|---|---|
| "No se pudo completar la operación. Verifique la conexión…" | SQL Server no está iniciado, la instancia no se llama `SQLEXPRESS` o falta la base. Revisá el servicio en *Servicios de Windows* y `App.config`. |
| El script falla en `CREATE DATABASE` | Versión de SQL Server distinta o ruta de datos incorrecta. Ver el paso 2. |
| Backup falla con "Acceso denegado" (error 5) | El servicio de SQL Server no tiene permiso sobre la carpeta de destino. Ver el paso 3. |
| Aviso "Falta el archivo credentials.json" | Es solo del backup en Drive. Descargá `credentials.json` del repo y copialo en `bin\Debug` (junto a `AorusMarket.exe`). Ver el Paso 6. |
| El backup automático no se ejecuta | Solo corre mientras la aplicación está abierta con una sesión de Administrador o Gerente, y dentro de las 2 horas posteriores a la hora programada. |
| No puedo restaurar un backup | Requiere perfil Administrador o Gerente y las credenciales de **otro** usuario con ese perfil. |
| Errores inesperados | Revisá los logs en `%LocalAppData%\AorusMarket\Logs`. |

---

Proyecto académico desarrollado por **ThKo55**.
