using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace apiRESTCheckUsuarioTADS.Models
{
    public class clsUsuario
    {
        public string cve { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string usuario { get; set; }
        public string contrasena { get; set; }
        public string ruta { get; set; }
        public int tipo { get; set; }

        private string cadConn
        {
            get
            {
                ConnectionStringSettings conexion =
                    ConfigurationManager.ConnectionStrings["bdControlAcceso"];

                if (conexion == null)
                {
                    throw new ConfigurationErrorsException(
                        "No se encontró la conexión bdControlAcceso en Web.config.");
                }

                return conexion.ConnectionString;
            }
        }

        public clsUsuario()
        {
        }

        public clsUsuario(string usuario, string contrasena)
        {
            this.usuario = usuario;
            this.contrasena = contrasena;
        }

        public clsUsuario(
            string nombre,
            string apellidoPaterno,
            string apellidoMaterno,
            string usuario,
            string contrasena,
            string ruta,
            int tipo)
        {
            this.nombre = nombre;
            this.apellidoPaterno = apellidoPaterno;
            this.apellidoMaterno = apellidoMaterno;
            this.usuario = usuario;
            this.contrasena = contrasena;
            this.ruta = ruta;
            this.tipo = tipo;
        }

        // Registro de usuario
        public DataSet spInsUsuario()
        {
            string cadSQL = @"
                CALL spInsUsuario(
                    @nombre,
                    @paterno,
                    @materno,
                    @usuario,
                    @contrasena,
                    @ruta,
                    @tipo
                );";

            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand(cadSQL, cnn))
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@nombre", MySqlDbType.VarChar, 20)
                    .Value = (object)nombre ?? DBNull.Value;

                cmd.Parameters.Add("@paterno", MySqlDbType.VarChar, 20)
                    .Value = (object)apellidoPaterno ?? DBNull.Value;

                cmd.Parameters.Add("@materno", MySqlDbType.VarChar, 20)
                    .Value = (object)apellidoMaterno ?? DBNull.Value;

                cmd.Parameters.Add("@usuario", MySqlDbType.VarChar, 20)
                    .Value = (object)usuario ?? DBNull.Value;

                cmd.Parameters.Add("@contrasena", MySqlDbType.VarChar, 20)
                    .Value = (object)contrasena ?? DBNull.Value;

                cmd.Parameters.Add("@ruta", MySqlDbType.VarChar, 200)
                    .Value = (object)ruta ?? DBNull.Value;

                cmd.Parameters.Add("@tipo", MySqlDbType.Int32)
                    .Value = tipo;

                DataSet ds = new DataSet();
                da.Fill(ds, "spInsUsuario");

                return ds;
            }
        }

        // Validación de usuario y contraseña
        public DataSet spValidarAcceso()
        {
            string cadSQL = @"
                CALL spValidarAcceso(@usuario, @contrasena);";

            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand(cadSQL, cnn))
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@usuario", MySqlDbType.VarChar, 20)
                    .Value = (object)usuario ?? DBNull.Value;

                cmd.Parameters.Add("@contrasena", MySqlDbType.VarChar, 20)
                    .Value = (object)contrasena ?? DBNull.Value;

                DataSet ds = new DataSet();
                da.Fill(ds, "spValidarAcceso");

                return ds;
            }
        }

        // Reporte con filtro por nombre, apellidos o usuario
        public DataSet vwRptUsuario(string filtro = "")
        {
            string cadSQL = @"
                SELECT *
                FROM vwrptusuario
                WHERE Nombre LIKE @filtro
                   OR Usuario LIKE @filtro;";

            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand(cadSQL, cnn))
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@filtro", MySqlDbType.VarChar)
                    .Value = "%" + (filtro ?? "").Trim() + "%";

                DataSet ds = new DataSet();
                da.Fill(ds, "vwRptUsuario");

                return ds;
            }
        }

        // Catálogo de tipos de usuario
        public DataSet vwTipoUsuario()
        {
            string cadSQL = @"
                SELECT clave, descripcion
                FROM vwtipousuario
                ORDER BY clave;";

            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand(cadSQL, cnn))
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds, "vwTipoUsuario");

                return ds;
            }
        }
    }
}