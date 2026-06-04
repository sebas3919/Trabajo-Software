using Newtonsoft.Json;
using TrabajoFinalWPF.Models;

namespace TrabajoFinalWPF.Services;

public class UsuarioService
{
    // FIX: Ruta fija junto al ejecutable para que los datos no "desaparezcan"
    // entre ejecuciones desde Visual Studio vs. desde el explorador de archivos.
    private static readonly string Archivo = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "usuarios.json");

    public List<Usuario> ObtenerUsuarios()
    {
        if (!File.Exists(Archivo)) return new();
        var contenido = File.ReadAllText(Archivo);
        return JsonConvert.DeserializeObject<List<Usuario>>(contenido) ?? new();
    }

    public void GuardarUsuarios(List<Usuario> usuarios) =>
        File.WriteAllText(Archivo, JsonConvert.SerializeObject(usuarios, Formatting.Indented));

    public Usuario? Buscar(string gmail) =>
        ObtenerUsuarios()
            .FirstOrDefault(u => u.Gmail.Equals(gmail, StringComparison.OrdinalIgnoreCase));

    public void Actualizar(Usuario usuarioActualizado, string gmailOriginal)
    {
        var lista = ObtenerUsuarios();
        var idx = lista.FindIndex(x =>
            x.Gmail.Equals(gmailOriginal, StringComparison.OrdinalIgnoreCase));
        if (idx >= 0)
        {
            lista[idx] = usuarioActualizado;
            GuardarUsuarios(lista);
        }
    }
}
