using System.Windows;
using TrabajoFinalWPF.Models;
using TrabajoFinalWPF.Services;

namespace TrabajoFinalWPF.Views;

public partial class DocenteWindow : Window
{
    // FIX: guardar la referencia al admin para poder usar su nombre y datos
    private readonly Usuario _admin;
    private readonly UsuarioService _service = new();

    public DocenteWindow(Usuario admin)
    {
        InitializeComponent();
        _admin = admin;
        // FIX: mostrar nombre del administrador en el sidebar
        txtNombreAdmin.Text = admin.Nombre;
        Cargar();
    }

    private void Cargar()
    {
        dg.ItemsSource = _service.ObtenerUsuarios()
            .Where(u => u.TipoUsuario == "Egresado")
            .ToList();
    }

    private void Agregar_Click(object sender, RoutedEventArgs e)
    {
        new AgregarEstudianteWindow().ShowDialog();
        Cargar(); // recargar tabla tras agregar
    }
}
