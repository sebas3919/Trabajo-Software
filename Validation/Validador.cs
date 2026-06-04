using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TrabajoFinalWPF.Validation;

/// <summary>
/// Validador de formularios para WPF.
/// Expone también MostrarError / QuitarError como utilidades estáticas
/// para que todas las ventanas los reutilicen sin duplicar código.
/// </summary>
public class Validador
{
    // ── Reglas de validación ──────────────────────────────────────────────────
    private readonly Dictionary<string, (string Regex, string Mensaje)> _reglas = new()
    {
        { "Nombre",   (@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ][a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$",
                       "El nombre debe tener al menos 2 caracteres y solo letras") },

        { "Correo",   (@"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z]{2,})+$",
                       "Ingrese un correo válido (ej: nombre@udenar.edu.co)") },

        { "Password", (@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[^a-zA-Z0-9]).{6,}$",
                       "La contraseña debe tener mínimo 6 caracteres, una mayúscula, una minúscula y un carácter especial") },

        { "Cedula",   (@"^1\d{9}$",
                       "La cédula debe iniciar en 1 y tener 10 dígitos") },

        { "Telefono", (@"^3\d{9}$",
                       "El teléfono debe iniciar en 3 y tener 10 dígitos") },

        { "Usuario",  (@"^[a-zA-Z]{5,}$",
                       "El usuario debe tener al menos 5 letras sin espacios ni números") },
    };

    // ── Colores compartidos ───────────────────────────────────────────────────
    public static readonly SolidColorBrush ErrorBrush =
        new(Color.FromRgb(0xFF, 0xCC, 0xCC));
    public static readonly SolidColorBrush OkBrush =
        new(Colors.White);
    private static readonly SolidColorBrush ErrorTextBrush =
        new(Color.FromRgb(0xCC, 0x00, 0x00));

    // ── API pública ───────────────────────────────────────────────────────────

    /// <summary>
    /// Valida todos los TextBox y PasswordBox dentro del contenedor.
    /// Devuelve true si todos son válidos.
    /// </summary>
    public bool ValidarFormulario(Panel contenedor)
    {
        bool valido = true;

        foreach (var control in ObtenerControlesValidables(contenedor))
        {
            string texto = ObtenerTexto(control);
            string tag   = (control as FrameworkElement)?.Tag?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(texto))
            {
                MostrarErrorEnPanel(control,
                    $"El campo {(string.IsNullOrEmpty(tag) ? "requerido" : tag)} es obligatorio");
                valido = false;
                continue;
            }

            if (string.IsNullOrEmpty(tag) || !_reglas.ContainsKey(tag))
            {
                QuitarErrorEnPanel(control);
                continue;
            }

            var (pattern, mensaje) = _reglas[tag];
            if (!Regex.IsMatch(texto, pattern))
            {
                MostrarErrorEnPanel(control, mensaje);
                valido = false;
            }
            else
            {
                QuitarErrorEnPanel(control);
            }
        }

        return valido;
    }

    /// <summary>Limpia todos los errores visuales del contenedor.</summary>
    public void LimpiarErrores(Panel contenedor)
    {
        foreach (var control in ObtenerControlesValidables(contenedor))
            QuitarErrorEnPanel(control);

        var etiquetas = new List<UIElement>();
        foreach (UIElement hijo in contenedor.Children)
            if (hijo is TextBlock tb && tb.Tag?.ToString() == "ErrorLabel")
                etiquetas.Add(hijo);

        foreach (var e in etiquetas)
            contenedor.Children.Remove(e);
    }

    // ── Utilidades estáticas reutilizables por cualquier ventana ─────────────
    // FIX: antes estas 2 funciones estaban duplicadas en LoginWindow,
    // AgregarEstudianteWindow y RegistroAdministradorWindow.

    /// <summary>Marca un campo y su etiqueta de error como inválidos.</summary>
    public static void MostrarError(Control campo, TextBlock label, string mensaje)
    {
        campo.Background = ErrorBrush;
        label.Text       = mensaje;
        label.Visibility = Visibility.Visible;
    }

    /// <summary>Quita el estado de error de un campo y su etiqueta.</summary>
    public static void QuitarError(Control campo, TextBlock label)
    {
        campo.Background = OkBrush;
        label.Text       = "";
        label.Visibility = Visibility.Collapsed;
    }

    // ── Helpers privados (para ValidarFormulario) ─────────────────────────────

    private static string ObtenerTexto(UIElement control) => control switch
    {
        TextBox tb     => tb.Text,
        PasswordBox pb => pb.Password,
        _              => ""
    };

    private static List<UIElement> ObtenerControlesValidables(Panel panel)
    {
        var lista = new List<UIElement>();
        foreach (UIElement hijo in panel.Children)
        {
            if (hijo is TextBox || hijo is PasswordBox)
                lista.Add(hijo);

            if (hijo is Panel subPanel)
                lista.AddRange(ObtenerControlesValidables(subPanel));

            if (hijo is ContentControl cc && cc.Content is Panel innerPanel)
                lista.AddRange(ObtenerControlesValidables(innerPanel));
        }
        return lista;
    }

    private void MostrarErrorEnPanel(UIElement control, string mensaje)
    {
        if (control is Control c) c.Background = ErrorBrush;

        if (control is FrameworkElement fe && fe.Parent is Panel parent)
        {
            string errorTag = "Error_" + fe.Name;
            TextBlock? errorLabel = null;

            foreach (UIElement hijo in parent.Children)
                if (hijo is TextBlock tb && tb.Tag?.ToString() == errorTag)
                { errorLabel = tb; break; }

            if (errorLabel is null)
            {
                errorLabel = new TextBlock
                {
                    Tag          = errorTag,
                    Foreground   = ErrorTextBrush,
                    FontSize     = 11,
                    Margin       = new Thickness(0, -4, 0, 6),
                    TextWrapping = TextWrapping.Wrap
                };
                int idx = parent.Children.IndexOf(fe);
                parent.Children.Insert(idx + 1, errorLabel);
            }

            errorLabel.Text = mensaje;
        }
    }

    private static void QuitarErrorEnPanel(UIElement control)
    {
        if (control is Control c) c.Background = OkBrush;

        if (control is FrameworkElement fe && fe.Parent is Panel parent)
        {
            string errorTag = "Error_" + fe.Name;
            UIElement? toRemove = null;

            foreach (UIElement hijo in parent.Children)
                if (hijo is TextBlock tb && tb.Tag?.ToString() == errorTag)
                { toRemove = hijo; break; }

            if (toRemove is not null)
                parent.Children.Remove(toRemove);
        }
    }
}
