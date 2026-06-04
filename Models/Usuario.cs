namespace TrabajoFinalWPF.Models;

public class Usuario
{
    public string Nombre { get; set; } = "";
    public string Gmail { get; set; } = "";
    public string Password { get; set; } = "";
    public string TipoUsuario { get; set; } = "";
    public string CodigoEstudiantil { get; set; } = "";
    public List<CuestionarioResultado> Cuestionarios { get; set; } = new();
    public string NivelDesercionAcademica { get; set; } = "Bajo";
}
