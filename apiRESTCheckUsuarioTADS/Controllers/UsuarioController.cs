using System;
using System.Data;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using apiRESTCheckUsuarioTADS.Models;

namespace apiRESTCheckUsuarioTADS.Controllers
{
    public class UsuarioController : ApiController
    {
        // POST: tads/usuario/spinsusuario
        [HttpPost]
        [Route("tads/usuario/spinsusuario")]
        public clsApiStatus spInsUsuario([FromBody] clsUsuario modelo)
        {
            clsApiStatus respuesta = new clsApiStatus();

            try
            {
                if (modelo == null || !ModelState.IsValid)
                {
                    respuesta.statusExec = false;
                    respuesta.ban = -1;
                    respuesta.msg = "Los datos enviados no son válidos.";
                    respuesta.datos = new JObject
                    {
                        ["msgData"] = "Envía un cuerpo JSON válido."
                    };

                    return respuesta;
                }

                clsUsuario objUsuario = new clsUsuario(
                    modelo.nombre,
                    modelo.apellidoPaterno,
                    modelo.apellidoMaterno,
                    modelo.usuario,
                    modelo.contrasena,
                    modelo.ruta,
                    modelo.tipo
                );

                DataSet ds = objUsuario.spInsUsuario();

                if (ds.Tables.Count == 0 ||
                    ds.Tables[0].Rows.Count == 0 ||
                    ds.Tables[0].Columns.Count == 0)
                {
                    throw new InvalidOperationException(
                        "spInsUsuario no devolvió la bandera de resultado.");
                }

                int bandera = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                string mensaje;

                switch (bandera)
                {
                    case 0:
                        mensaje = "Usuario registrado exitosamente.";
                        break;

                    case 1:
                        mensaje = "La persona ya está registrada.";
                        break;

                    case 2:
                        mensaje = "El nombre de usuario ya existe.";
                        break;

                    case 3:
                        mensaje = "El tipo de usuario no existe.";
                        break;

                    default:
                        mensaje =
                            "El procedimiento devolvió un resultado desconocido.";
                        break;
                }

                respuesta.statusExec = bandera == 0;
                respuesta.ban = bandera;
                respuesta.msg = mensaje;
                respuesta.datos = new JObject
                {
                    ["msgData"] = mensaje
                };
            }
            catch (Exception ex)
            {
                respuesta.statusExec = false;
                respuesta.ban = -1;
                respuesta.msg = "No se pudo registrar al usuario.";
                respuesta.datos = new JObject
                {
                    ["msgData"] = ex.Message
                };
            }

            return respuesta;
        }

        // POST: tads/usuario/spvalidaracceso
        [HttpPost]
        [Route("tads/usuario/spvalidaracceso")]
        public clsApiStatus spValidarAcceso([FromBody] clsUsuario modelo)
        {
            clsApiStatus respuesta = new clsApiStatus();

            try
            {
                if (modelo == null ||
                    !ModelState.IsValid ||
                    string.IsNullOrWhiteSpace(modelo.usuario) ||
                    string.IsNullOrWhiteSpace(modelo.contrasena))
                {
                    respuesta.statusExec = false;
                    respuesta.ban = -1;
                    respuesta.msg = "Debes enviar usuario y contraseña.";
                    respuesta.datos = new JObject
                    {
                        ["msgData"] = "Envía un JSON con usuario y contrasena."
                    };

                    return respuesta;
                }

                clsUsuario objUsuario = new clsUsuario(
                    modelo.usuario,
                    modelo.contrasena
                );

                DataSet ds = objUsuario.spValidarAcceso();

                if (ds.Tables.Count == 0 ||
                    ds.Tables[0].Rows.Count == 0 ||
                    !ds.Tables[0].Columns.Contains("resultado"))
                {
                    throw new InvalidOperationException(
                        "spValidarAcceso no devolvió la columna resultado.");
                }

                DataTable tabla = ds.Tables[0];
                int resultado =
                    Convert.ToInt32(tabla.Rows[0]["resultado"]);

                if (resultado != 0 && resultado != 1)
                {
                    throw new InvalidOperationException(
                        "spValidarAcceso devolvió un resultado desconocido.");
                }

                respuesta.statusExec = resultado == 1;
                respuesta.ban = resultado;
                respuesta.msg = resultado == 1
                    ? "Acceso validado exitosamente."
                    : "Usuario o contraseña incorrectos.";

                respuesta.datos = new JObject
                {
                    ["spValidarAcceso"] = resultado == 1
                        ? JArray.FromObject(tabla)
                        : new JArray()
                };
            }
            catch (Exception ex)
            {
                respuesta.statusExec = false;
                respuesta.ban = -1;
                respuesta.msg = "Falló la validación de acceso.";
                respuesta.datos = new JObject
                {
                    ["msgData"] = ex.Message
                };
            }

            return respuesta;
        }

        // GET: tads/usuario/vwrptusuario?filtro=texto
        [HttpGet]
        [Route("tads/usuario/vwrptusuario")]
        public clsApiStatus vwRptUsuario(string filtro = "")
        {
            clsApiStatus respuesta = new clsApiStatus();

            try
            {
                clsUsuario objUsuario = new clsUsuario();
                DataSet ds = objUsuario.vwRptUsuario(filtro);

                DataTable tabla = ds.Tables["vwRptUsuario"];

                if (tabla == null)
                {
                    throw new InvalidOperationException(
                        "La consulta no devolvió la tabla vwRptUsuario.");
                }

                respuesta.statusExec = true;
                respuesta.ban = tabla.Rows.Count;
                respuesta.msg =
                    "Consulta de usuarios realizada exitosamente.";

                respuesta.datos = new JObject
                {
                    ["vwRptUsuario"] = JArray.FromObject(tabla)
                };
            }
            catch (Exception ex)
            {
                respuesta.statusExec = false;
                respuesta.ban = -1;
                respuesta.msg =
                    "Falló la consulta del reporte de usuarios.";

                respuesta.datos = new JObject
                {
                    ["msgData"] = ex.Message
                };
            }

            return respuesta;
        }

        // GET: tads/usuario/vwtipousuario
        [HttpGet]
        [Route("tads/usuario/vwtipousuario")]
        public clsApiStatus vwTipoUsuario()
        {
            clsApiStatus respuesta = new clsApiStatus();

            try
            {
                clsUsuario objUsuario = new clsUsuario();
                DataSet ds = objUsuario.vwTipoUsuario();

                DataTable tabla = ds.Tables["vwTipoUsuario"];

                if (tabla == null)
                {
                    throw new InvalidOperationException(
                        "La consulta no devolvió la tabla vwTipoUsuario.");
                }

                respuesta.statusExec = true;
                respuesta.ban = tabla.Rows.Count;
                respuesta.msg =
                    "Consulta de tipos de usuario realizada exitosamente.";

                respuesta.datos = new JObject
                {
                    ["vwTipoUsuario"] = JArray.FromObject(tabla)
                };
            }
            catch (Exception ex)
            {
                respuesta.statusExec = false;
                respuesta.ban = -1;
                respuesta.msg =
                    "Falló la consulta de tipos de usuario.";

                respuesta.datos = new JObject
                {
                    ["msgData"] = ex.Message
                };
            }

            return respuesta;
        }
    }
}