using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using AorusMarket.AccesoDatos;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace AorusMarket.Negocio
{
    public class BackupNegocio
    {
        public bool GenerarRespaldo(string rutaCompleta, string tipoDestino, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                using (SqlConnection oConexion = Conexion.ObtenerConexion())
                {
                    oConexion.Open();
                    string nombreBd = oConexion.Database;

                    string query = $"BACKUP DATABASE [{nombreBd}] TO DISK = @ruta WITH FORMAT, MEDIANAME = 'AorusBackups', NAME = 'Backup Completo AorusMarket';";

                    using (SqlCommand cmd = new SqlCommand(query, oConexion))
                    {
                        cmd.Parameters.AddWithValue("@ruta", rutaCompleta);
                        cmd.CommandTimeout = 300;
                        cmd.ExecuteNonQuery();
                    }
                }

                // 1. Conserva solo los 5 más recientes
                string carpeta = Path.GetDirectoryName(rutaCompleta);
                PurgarBackupsAntiguos(carpeta, 5);

                // 2. Si se eligió Google Drive, sube el archivo
                if (tipoDestino.Contains("Google Drive"))
                {
                    SubirAGoogleDrive(rutaCompleta, out string msjDrive);
                    if (!string.IsNullOrEmpty(msjDrive))
                    {
                        mensaje = "Backup local creado. Aviso Drive: " + msjDrive;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error al generar el respaldo: " + ex.Message;
                return false;
            }
        }

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

        public bool RestaurarBaseDatos(string archivoBak, out string mensaje)
        {
            mensaje = string.Empty;

            if (!File.Exists(archivoBak))
            {
                mensaje = "El archivo de respaldo seleccionado no existe.";
                return false;
            }

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Conexion.ObtenerConexion().ConnectionString);
                string nombreDbActual = builder.InitialCatalog;
                builder.InitialCatalog = "master";

                using (SqlConnection oConexionMaster = new SqlConnection(builder.ConnectionString))
                {
                    oConexionMaster.Open();

                    string querySingleUser = $"ALTER DATABASE [{nombreDbActual}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                    using (SqlCommand cmd1 = new SqlCommand(querySingleUser, oConexionMaster))
                    {
                        cmd1.ExecuteNonQuery();
                    }

                    string queryRestore = $"RESTORE DATABASE [{nombreDbActual}] FROM DISK = @ruta WITH REPLACE;";
                    using (SqlCommand cmd2 = new SqlCommand(queryRestore, oConexionMaster))
                    {
                        cmd2.Parameters.AddWithValue("@ruta", archivoBak);
                        cmd2.CommandTimeout = 300;
                        cmd2.ExecuteNonQuery();
                    }

                    string queryMultiUser = $"ALTER DATABASE [{nombreDbActual}] SET MULTI_USER;";
                    using (SqlCommand cmd3 = new SqlCommand(queryMultiUser, oConexionMaster))
                    {
                        cmd3.ExecuteNonQuery();
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
    }
}