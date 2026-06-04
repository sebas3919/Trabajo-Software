using System.Text.RegularExpressions;
using System.Windows;
using TrabajoFinalWPF.Models;
using TrabajoFinalWPF.Services;
using TrabajoFinalWPF.Validation;

namespace TrabajoFinalWPF.Views;

public partial class AgregarEstudianteWindow : Window
{
    private readonly UsuarioService _service = new();

    private static readonly Regex RegexNombre =
        new(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ][a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$");

    private static readonly Regex RegexCorreo =
        new(@"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z]{2,})+$",
            RegexOptions.IgnoreCase);

    public AgregarEstudianteWindow() => InitializeComponent();

    // FIX: usar GUID en lugar de DateTime.Ticks para garantizar unicidad del código
    private static string GenerarCodigo() =>
        "COD" + Guid.NewGuid().ToString("N")[..8].ToUpper();

    private void Crear_Click(object sender, RoutedEventArgs e)
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

        if (!valido) return;

        var usuarios = _service.ObtenerUsuarios();
        if (usuarios.Any(u => u.Gmail.Equals(correo, StringComparison.OrdinalIgnoreCase)))
        {
            Validador.MostrarError(txtCorreo, lblErrorCorreo,
                "Este correo ya está registrado en el sistema");
            return;
        }

        string codigo = GenerarCodigo();
        usuarios.Add(new Usuario
        {
            Nombre                  = nombre,
            Gmail                   = correo,
            TipoUsuario             = "Egresado",
            CodigoEstudiantil       = codigo,
            NivelDesercionAcademica = "Bajo"
        });

        _service.GuardarUsuarios(usuarios);

        MessageBox.Show(
            $"Egresado creado correctamente.\n\nCódigo de acceso: {codigo}\n\nGuarda este código, el egresado lo necesitará para iniciar sesión.",
            "Egresado registrado", MessageBoxButton.OK, MessageBoxImage.Information);

        Close();
    }
}
