using System.Text.RegularExpressions;
using System.Windows;
using TrabajoFinalWPF.Services;
using TrabajoFinalWPF.Validation;

namespace TrabajoFinalWPF.Views;

public partial class LoginWindow : Window
{
    private readonly AuthService _auth = new();

    private static readonly Regex RegexCorreo =
        new(@"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z]{2,})+$",
            RegexOptions.IgnoreCase);

    public LoginWindow() => InitializeComponent();

    // ── Inicio de sesión ──────────────────────────────────────────────────────
    private void Login_Click(object sender, RoutedEventArgs e)
    {
        string correo = txtCorreo.Text.Trim();
        string clave  = txtClave.Password.Trim();
        bool valido   = true;

        if (string.IsNullOrWhiteSpace(correo))
        {
            Validador.MostrarError(txtCorreo, lblErrorCorreo, "El correo es obligatorio");
            valido = false;
        }
        else if (!RegexCorreo.IsMatch(correo))
        {
            Validador.MostrarError(txtCorreo, lblErrorCorreo,
                "Ingrese un correo válido (ej: usuario@udenar.edu.co)");
            valido = false;
        }
        else Validador.QuitarError(txtCorreo, lblErrorCorreo);

        if (string.IsNullOrWhiteSpace(clave))
        {
            Validador.MostrarError(txtClave, lblErrorClave,
                "La contraseña o código es obligatorio");
            valido = false;
        }
        else Validador.QuitarError(txtClave, lblErrorClave);

        if (!valido) return;

        var user = _auth.Login(correo, clave);
        if (user is null)
        {
            MessageBox.Show(
                "Credenciales inválidas. Verifique su correo y contraseña/código.",
                "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (user.TipoUsuario == "Administrador")
            // FIX: pasar el objeto admin a DocenteWindow para que pueda usarlo
            new DocenteWindow(user).Show();
        else
            new PanelUsuarioWindow(user).Show();

        Close();
    }

    // ── Abrir registro de administrador ──────────────────────────────────────
    private void RegistroAdmin_Click(object sender, RoutedEventArgs e)
    {
        string codigo = txtCodigoAdmin.Text.Trim();

        if (string.IsNullOrWhiteSpace(codigo))
        {
            Validador.MostrarError(txtCodigoAdmin, lblErrorCodigo,
                "Ingrese el código administrativo");
            return;
        }
        if (codigo != AuthService.CodigoAdmin)
        {
            Validador.MostrarError(txtCodigoAdmin, lblErrorCodigo,
                "Código administrativo incorrecto");
            return;
        }

        Validador.QuitarError(txtCodigoAdmin, lblErrorCodigo);
        new RegistroAdministradorWindow().ShowDialog();
    }
}
