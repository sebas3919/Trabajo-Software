using System.Windows;
using TrabajoFinalWPF.Models;
using TrabajoFinalWPF.Services;

namespace TrabajoFinalWPF.Views;

public partial class PanelUsuarioWindow : Window
{
    // FIX: recargar datos frescos desde el archivo para reflejar cambios
    // que el admin haya hecho después del login del egresado.
    public PanelUsuarioWindow(Usuario userRecibido)
    {
        InitializeComponent();

        // Buscar versión actualizada en el archivo
        var service = new UsuarioService();
        var user = service.Buscar(userRecibido.Gmail) ?? userRecibido;

        txtBienvenida.Text     = $"Bienvenido, {user.Nombre}";
        txtNivel.Text          = user.NivelDesercionAcademica;
        dgCuestionarios.ItemsSource = user.Cuestionarios;
    }
}
