using AorusMarket.AccesoDatos;
using AorusMarket.Entidades;
using AorusMarket.Utilidades;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AorusMarket.Negocio
{
    public class BackupNegocio
    {

        private BackupDatos objDatos = new BackupDatos();

        public void PurgarBackupsAntiguos(string carpeta, int cantidadMaxima)
        {
            try
            {
                if (!Directory.Exists(carpeta)) return;

                DirectoryInfo info = new DirectoryInfo(carpeta);
                var archivos = info.GetFiles("AorusMarket_*.bak")
                                   .OrderByDescending(f => f.CreationTime)
                                   .ToList();

                if (archivos.Count > cantidadMaxima)
                {
                    var aEliminar = archivos.Skip(cantidadMaxima);
                    foreach (var archivo in aEliminar)
                    {
                        try { archivo.Delete(); } catch { }
                    }
                }
            }
            catch { }
        }

        public bool SubirAGoogleDrive(string rutaArchivoBak, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                string credencialesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credentials.json");

                if (!File.Exists(credencialesPath))
                {
                    mensaje = "Falta el archivo credentials.json en la carpeta del sistema.";
                    return false;
                }

                UserCredential credential;
                using (var stream = new FileStream(credencialesPath, FileMode.Open, FileAccess.Read))
                {
                    string tokenPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DriveToken");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        new[] { DriveService.Scope.DriveFile },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(tokenPath, true)).Result;
                }

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "AorusMarket"
                });

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(rutaArchivoBak)
                };

                FilesResource.CreateMediaUpload request;
                using (var uploadStream = new FileStream(rutaArchivoBak, FileMode.Open))
                {
                    request = service.Files.Create(fileMetadata, uploadStream, "application/octet-stream");
                    request.Upload();
                }

                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error al subir a Google Drive: " + ex.Message;
                return false;
            }
        }


        private readonly BackupDatos _datos = new BackupDatos();

        public ConfigBackup ObtenerConfig() => _datos.ObtenerConfig();
        public void GuardarConfig(ConfigBackup cfg) => _datos.GuardarConfig(cfg);
        public DataTable ListarRegistro() => _datos.ListarRegistro();

        // El registro nunca debe romper la operación principal
        private void RegistrarSeguro(string tipo, string origen, string archivo,
                                     int? idSol, int? idApr, string estado, string detalle)
        {
            try { _datos.RegistrarOperacion(tipo, origen, archivo, idSol, idApr, estado, detalle); }
            catch { }

            Auditoria.Registrar($"{tipo} {origen.ToLower()} - {estado}", "Backup", archivo);
        }

        private static void EjecutarSql(SqlConnection cn, string sql)
        {
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.CommandTimeout = 300;
                cmd.ExecuteNonQuery();
            }
        }

        private void EjecutarBackupSql(string rutaCompleta)
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            {
                cn.Open();
                string bd = cn.Database;
                using (SqlCommand cmd = new SqlCommand(
                    $"BACKUP DATABASE [{bd}] TO DISK = @ruta WITH FORMAT, CHECKSUM, MEDIANAME = 'AorusBackups', NAME = 'Backup Completo AorusMarket';", cn))
                {
                    cmd.Parameters.AddWithValue("@ruta", rutaCompleta);
                    cmd.CommandTimeout = 300;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Comprueba que el archivo generado sea legible y no esté corrupto
        private void VerificarBackup(string rutaCompleta)
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("RESTORE VERIFYONLY FROM DISK = @ruta WITH CHECKSUM;", cn))
                {
                    cmd.Parameters.AddWithValue("@ruta", rutaCompleta);
                    cmd.CommandTimeout = 300;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool GenerarRespaldo(string rutaCompleta, string tipoDestino, int conservar, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                EjecutarBackupSql(rutaCompleta);
                VerificarBackup(rutaCompleta);

                PurgarBackupsAntiguos(Path.GetDirectoryName(rutaCompleta), conservar);

                if (tipoDestino.Contains("Google Drive"))
                {
                    SubirAGoogleDrive(rutaCompleta, out string msjDrive);
                    if (!string.IsNullOrEmpty(msjDrive))
                        mensaje = "Backup local creado. Aviso Drive: " + msjDrive;
                }
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error al generar el respaldo: " + ex.Message;
                return false;
            }
        }

        // Backup libre: lo puede ejecutar cualquier usuario en cualquier momento
        public bool CrearBackup(string carpeta, string tipoDestino, int conservar, string origen,
                                out string mensaje, out string rutaCompleta)
        {
            mensaje = string.Empty;
            rutaCompleta = string.Empty;
            int? idUsuario = SesionActual.IdUsuario > 0 ? SesionActual.IdUsuario : (int?)null;

            try
            {
                if (string.IsNullOrWhiteSpace(carpeta)) throw new InvalidOperationException("La ruta de destino está vacía.");
                if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
            }
            catch (Exception ex)
            {
                mensaje = "La ruta no es válida o no tiene permisos: " + ex.Message;
                RegistrarSeguro("Backup", origen, null, idUsuario, null, "Fallido", mensaje);
                return false;
            }

            rutaCompleta = Path.Combine(carpeta, $"AorusMarket_{DateTime.Now:yyyyMMdd_HHmmss}.bak");

            bool ok = GenerarRespaldo(rutaCompleta, tipoDestino, conservar, out mensaje);

            RegistrarSeguro("Backup", origen, ok ? rutaCompleta : null, idUsuario, null,
                            ok ? "Completado" : "Fallido", mensaje);

            if (ok)
            {
                try { _datos.ActualizarUltimoBackup(DateTime.Now); } catch { }
            }
            return ok;
        }

        // Restauración: antes de pisar la BD se guarda una copia del estado actual
        public bool Restaurar(string archivoBak, int idSolicitante, int idAprobador, out string mensaje)
        {
            mensaje = string.Empty;

            if (!File.Exists(archivoBak))
            {
                mensaje = "El archivo de respaldo seleccionado no existe.";
                return false;
            }

            try
            {
                string carpeta = Path.GetDirectoryName(archivoBak);
                string copiaPrevia = Path.Combine(carpeta, $"PreRestore_AorusMarket_{DateTime.Now:yyyyMMdd_HHmmss}.bak");
                EjecutarBackupSql(copiaPrevia);
            }
            catch (Exception ex)
            {
                mensaje = "No se pudo generar la copia previa de seguridad. Restauración cancelada: " + ex.Message;
                return false;
            }

            bool ok = RestaurarBaseDatos(archivoBak, out mensaje);

            // Se registra DESPUÉS: el restore reemplaza la BD (y con ella el registro)
            SqlConnection.ClearAllPools();
            RegistrarSeguro("Restauración", "Manual", archivoBak, idSolicitante, idAprobador,
                            ok ? "Completado" : "Fallido", ok ? null : mensaje);
            return ok;
        }

        public bool RestaurarBaseDatos(string archivoBak, out string mensaje)
        {
            mensaje = string.Empty;

            if (!File.Exists(archivoBak))
            {
                mensaje = "El archivo de respaldo seleccionado no existe.";
                return false;
            }

            string nombreDb, cadenaMaster;
            using (SqlConnection tmp = Conexion.ObtenerConexion())
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(tmp.ConnectionString);
                nombreDb = builder.InitialCatalog;
                builder.InitialCatalog = "master";
                cadenaMaster = builder.ConnectionString;
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(cadenaMaster))
                {
                    cn.Open();
                    EjecutarSql(cn, $"ALTER DATABASE [{nombreDb}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            $"RESTORE DATABASE [{nombreDb}] FROM DISK = @ruta WITH REPLACE;", cn))
                        {
                            cmd.Parameters.AddWithValue("@ruta", archivoBak);
                            cmd.CommandTimeout = 300;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    finally
                    {
                        // Si el RESTORE falla, la BD NO queda bloqueada en modo single user
                        EjecutarSql(cn, $"ALTER DATABASE [{nombreDb}] SET MULTI_USER;");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error al restaurar: " + ex.Message;
                return false;
            }
        }

        public async Task EjecutarBackupAutomaticoAsync(string rutaDestino)
        {
            await objDatos.EjecutarBackupAutomaticoAsync(rutaDestino);
        }




    }
}