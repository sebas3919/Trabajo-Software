using System.Text.RegularExpressions;
using TrabajoFinalWPF.Models;

namespace TrabajoFinalWPF.Services;

public class AuthService
{
    public const string CodigoAdmin = "ABC123";

    private static readonly Regex RegexCorreo =
        new(@"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z]{2,})+$",
            RegexOptions.IgnoreCase);

    private readonly UsuarioService _service = new();

    /// <summary>
    /// Registra un administrador. El código ya fue validado en la vista;
    /// aquí solo se verifica duplicidad de correo y formato.
    /// </summary>
    public bool RegistrarAdministrador(Usuario admin)
    {
        if (!RegexCorreo.IsMatch(admin.Gmail)) return false;

        var usuarios = _service.ObtenerUsuarios();
        bool correoRepetido = usuarios.Any(u =>
            u.Gmail.Equals(admin.Gmail, StringComparison.OrdinalIgnoreCase));
        if (correoRepetido) return false;

        usuarios.Add(admin);
        _service.GuardarUsuarios(usuarios);
        return true;
    }

    /// <summary>
    /// Autentica un usuario por correo y clave.
    /// Admin  → usa Password.
    /// Egresado → usa CodigoEstudiantil.
    /// Devuelve null si las credenciales son inválidas.
    /// </summary>
    public Usuario? Login(string correo, string clave)
    {
        if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(clave))
            return null;

        var user = _service.Buscar(correo);
        if (user is null) return null;

        return user.TipoUsuario switch
        {
            "Administrador" when user.Password == clave => user,
            "Egresado"      when user.CodigoEstudiantil == clave => user,
            _ => null
        };
    }
}
