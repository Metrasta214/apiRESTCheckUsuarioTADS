# apiRESTCheckUsuarioTADS

## Datos académicos

| Dato | Información |
|---|---|
| Institución | Tecnológico Nacional de México |
| Plantel | Instituto Tecnológico de Pachuca |
| Carrera | Ingeniería en Tecnologías de la Información y Comunicaciones |
| Materia | Taller Avanzado de Desarrollo de Software |
| Alumno | Álvaro Madrid Morales |
| Práctica | Web API para gestión de usuarios |
| Fecha | 13 de octubre de 2026 |

## Descripción

Web API desarrollada en C# con ASP.NET Web API 2 para gestionar usuarios mediante la base de datos MySQL `CONTROL_ACCESO`.

Permite registrar usuarios, validar sus credenciales, consultar un reporte con filtro y obtener el catálogo de tipos de usuario. Las consultas utilizan parámetros para enviar los datos a MySQL.

## Tecnologías utilizadas

- Visual Studio Community.
- C# y .NET Framework 4.7.2.
- ASP.NET Web API 2.
- MySQL y MySQL Workbench.
- MySql.Data.
- Newtonsoft.Json.
- Postman.

## Organización del proyecto

- `Models/clsApiStatus.cs`: define el formato de respuesta.
- `Models/clsUsuario.cs`: contiene los datos del usuario y las consultas a MySQL.
- `Controllers/UsuarioController.cs`: recibe las peticiones HTTP.
- `App_Start/WebApiConfig.cs`: configura las rutas de la API.
- `Web.config`: contiene la configuración de conexión.

## Configuración y ejecución

1. Clonar o descargar el repositorio.
2. Abrir el archivo de solución `.sln` en Visual Studio.
3. Restaurar los paquetes NuGet.
4. Tener disponible la base de datos `CONTROL_ACCESO` con estos objetos:
   - Procedimiento `spInsUsuario`.
   - Procedimiento `spValidarAcceso`.
   - Vista `vwrptusuario`.
   - Vista `vwtipousuario`.
5. Configurar la conexión `bdControlAcceso` en el archivo `Web.config`:

```xml
<connectionStrings>
  <add name="bdControlAcceso"
       connectionString="server=localhost;port=3306;database=CONTROL_ACCESO;uid=root;pwd=TU_PASSWORD;"
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

6. Sustituir `TU_PASSWORD` por la contraseña local de MySQL.
7. Compilar y ejecutar el proyecto con F5.
8. Utilizar la dirección y el puerto que abra Visual Studio para realizar las pruebas.

La contraseña real de MySQL no debe publicarse en el repositorio. La base de datos debe configurarse por separado.

## Endpoints

| Método | Ruta | Función |
|---|---|---|
| POST | `/tads/usuario/spinsusuario` | Registrar un usuario |
| POST | `/tads/usuario/spvalidaracceso` | Validar usuario y contraseña |
| GET | `/tads/usuario/vwrptusuario` | Consultar usuarios con filtro opcional |
| GET | `/tads/usuario/vwtipousuario` | Consultar tipos de usuario |

### Registrar usuario

**POST** `/tads/usuario/spinsusuario`

Enviar en Body → raw → JSON:

```json
{
  "nombre": "Alvaro",
  "apellidoPaterno": "Madrid",
  "apellidoMaterno": "Morales",
  "usuario": "amadrid_prueba",
  "contrasena": "Prueba123",
  "ruta": "imagenes/fotos/alvaro.jpg",
  "tipo": 3
}
```

Resultados del procedimiento:

| `ban` | Significado |
|---|---|
| 0 | Usuario registrado |
| 1 | Persona ya registrada |
| 2 | Nombre de usuario existente |
| 3 | Tipo de usuario inexistente |
| -1 | Error de ejecución o datos inválidos |

### Validar acceso

**POST** `/tads/usuario/spvalidaracceso`

Enviar en Body → raw → JSON:

```json
{
  "usuario": "amadrid_prueba",
  "contrasena": "Prueba123"
}
```

| `ban` | Significado |
|---|---|
| 1 | Credenciales correctas |
| 0 | Credenciales incorrectas |
| -1 | Error de ejecución o datos inválidos |

Cuando las credenciales son correctas, la API devuelve el nombre completo, la ruta de la foto, el usuario y la descripción de su tipo. Esta operación verifica credenciales; no genera una sesión ni un token.

### Consultar reporte de usuarios

**GET** `/tads/usuario/vwrptusuario`

Sin filtro, devuelve todos los usuarios disponibles en la vista.

Para buscar:

```text
/tads/usuario/vwrptusuario?filtro=Luis
/tads/usuario/vwrptusuario?filtro=Bell
/tads/usuario/vwrptusuario?filtro=Aguirre
/tads/usuario/vwrptusuario?filtro=dtvega
```

El filtro busca coincidencias parciales en `Nombre` y `Usuario`. El campo `Nombre` de la vista combina nombre, apellido paterno y apellido materno, por lo que permite buscar en los cuatro campos solicitados.

El reporte devuelve:

- `Clave`
- `Nombre`
- `Usuario`
- `Foto`
- `Rol`

Si no hay coincidencias, devuelve una lista vacía y `ban: 0`.

### Consultar tipos de usuario

**GET** `/tads/usuario/vwtipousuario`

Devuelve el catálogo ordenado por clave, con los campos:

- `clave`
- `descripcion`

## Formato de respuesta

Todos los endpoints utilizan el modelo `clsApiStatus`.

| Campo | Descripción |
|---|---|
| `statusExec` | Indica si la operación fue exitosa |
| `msg` | Mensaje del resultado |
| `ban` | Código de resultado o cantidad de registros |
| `datos` | Información devuelta en JSON |

Ejemplo de reporte sin coincidencias:

```json
{
  "statusExec": true,
  "msg": "Consulta de usuarios realizada exitosamente.",
  "ban": 0,
  "datos": {
    "vwRptUsuario": []
  }
}
```

En este proyecto, una respuesta HTTP 200 puede contener `statusExec: false`; por ello también debe revisarse el contenido del JSON.

## Pruebas en Postman

La colección `apiRESTCheckUsuarioTADS` reúne las peticiones para comprobar:

- Registro de usuario nuevo y rechazo de duplicados.
- Validación con credenciales correctas e incorrectas.
- Reporte completo.
- Filtro por nombre, apellido paterno, apellido materno y usuario.
- Consulta sin coincidencias.
- Consulta del catálogo de tipos de usuario.

Para repetir las pruebas, importar el archivo:

```text
apiRESTCheckUsuarioTADS.postman_collection.json
```

Ajustar el protocolo y el puerto de las peticiones a la dirección local del proyecto. El archivo JSON se entrega comprimido en ZIP conforme a la consigna.

## Conclusión

Con esta práctica aprendí a conectar una Web API con MySQL y a separar las consultas del modelo de los endpoints del controlador. Las pruebas en Postman me ayudaron a comprobar los resultados y a corregir errores, como los nombres de columnas utilizados en el filtro de usuarios.