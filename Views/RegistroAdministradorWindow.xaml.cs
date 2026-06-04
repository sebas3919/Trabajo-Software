using System.Text.RegularExpressions;
using System.Windows;
using TrabajoFinalWPF.Models;
using TrabajoFinalWPF.Services;
using TrabajoFinalWPF.Validation;

namespace TrabajoFinalWPF.Views;

public partial class RegistroAdministradorWindow : Window
{
    private readonly AuthService _auth = new();

    private static readonly Regex RegexNombre =
        new(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ][a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$");

    private static readonly Regex RegexCorreo =
        new(@"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z]{2,})+$",
            RegexOptions.IgnoreCase);

    private static readonly Regex RegexPassword =
        new(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[^a-zA-Z0-9]).{6,}$");

    public RegistroAdministradorWindow() => InitializeComponent();

    private void Registrar_Click(object sender, RoutedEventArgs e)
    {
        bool valido = true;

        string nombre = txtNombre.Text.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            Validador.MostrarError(txtNombre, lblErrorNombre, "El nombre es obligatorio");
            valido = false;
        }
        else if (!RegexNombre.IsMatch(nombre))
        {
            Validador.MostrarError(txtNombre, lblErrorNombre,
                "El nombre debe tener al menos 2 caracteres y solo letras");
            valido = false;
        }
        else Validador.QuitarError(txtNombre, lblErrorNombre);

        string correo = txtCorreo.Text.Trim();
        if (string.IsNullOrWhiteSpace(correo))
        {
            Validador.MostrarError(txtCorreo, lblErrorCorreo, "El correo es obligatorio");
            valido = false;
        }
        else if (!RegexCorreo.IsMatch(correo))
        {
            Validador.MostrarError(txtCorreo, lblErrorCorreo,
                "Ingrese un correo válido (ej: nombre@udenar.edu.co)");
            valido = false;
        }
        else Validador.QuitarError(txtCorreo, lblErrorCorreo);

        string password = txtPassword.Password.Trim();
        if (string.IsNullOrWhiteSpace(password))
        {
            Validador.MostrarError(txtPassword, lblErrorPassword, "La contraseña es obligatoria");
            valido = false;
        }
        else if (!RegexPassword.IsMatch(password))
        {
            Validador.MostrarError(txtPassword, lblErrorPassword,
                "Mínimo 6 caracteres, una mayúscula, una minúscula y un carácter especial");
            valido = false;
        }
        else Validador.QuitarError(txtPassword, lblErrorPassword);

        if (!valido) return;

        var admin = new Usuario
        {
            Nombre      = nombre,
            Gmail       = correo,
            Password    = password,
            TipoUsuario = "Administrador"
        };

        // FIX: ya no se pasa el código aquí; el código fue validado en LoginWindow
        // antes de abrir esta ventana. AuthService solo verifica duplicidad de correo.
        if (_auth.RegistrarAdministrador(admin))
        {
            MessageBox.Show("Administrador registrado correctamente.",
                "Registro exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        else
        {
            MessageBox.Show("No se pudo registrar. El correo ya existe o hubo un error.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
