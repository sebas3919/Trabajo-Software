namespace TrabajoFinalWPF.Models;

public class CuestionarioResultado
{
    public string Titulo { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.Now;
    public int Puntaje { get; set; }
    public string Observaciones { get; set; } = "";
}
