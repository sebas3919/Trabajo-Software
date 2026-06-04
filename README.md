# TrabajoFinalWPF — Versión Corregida

## Cambios realizados

### Bugs corregidos

| Archivo | Problema | Corrección |
|---|---|---|
| `UsuarioService.cs` | `usuarios.json` se guardaba en el directorio de trabajo variable, causando pérdida de datos entre ejecuciones desde VS | Ruta fija junto al ejecutable usando `AppDomain.CurrentDomain.BaseDirectory` |
| `AgregarEstudianteWindow.xaml.cs` | `GenerarCodigo()` usaba `DateTime.Ticks`, pudiendo generar códigos duplicados si se creaban dos egresados casi simultáneamente | Se reemplazó por `Guid.NewGuid()` que garantiza unicidad |
| `DocenteWindow.xaml.cs` | Recibía el objeto `admin` pero nunca lo guardaba ni usaba | Se guarda en `_admin` y se muestra el nombre en el sidebar |
| `AuthService.cs` + `RegistroAdministradorWindow.xaml.cs` | El código ABC123 se validaba dos veces: en la vista y en el servicio, generando confusión | El servicio ya no recibe ni valida el código; la vista lo valida antes de abrir la ventana de registro |
| `PanelUsuarioWindow.xaml.cs` | Solo usaba el objeto `Usuario` recibido al login; si el admin modificaba al egresado después, el panel no se actualizaba | Al abrir el panel se recarga el usuario desde el archivo por su correo |

### Eliminación de código duplicado

`MostrarError()` y `QuitarError()` estaban copiadas en 3 ventanas diferentes:
- `LoginWindow.xaml.cs`
- `AgregarEstudianteWindow.xaml.cs`
- `RegistroAdministradorWindow.xaml.cs`

**Corrección:** Se convirtieron en métodos `public static` dentro de `Validador.cs`. Todas las ventanas ahora llaman `Validador.MostrarError(...)` y `Validador.QuitarError(...)`.

---

## Flujo del sistema
- Solo administradores pueden registrarse usando el código ABC123
- Los egresados NO se registran por cuenta propia
- Los administradores crean egresados manualmente desde el Panel Admin
- Cada egresado recibe un código único generado con GUID
- Los egresados ingresan con correo + código
