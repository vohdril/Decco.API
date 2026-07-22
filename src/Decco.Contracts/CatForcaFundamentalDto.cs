namespace Decco.Contracts;

public class CatForcaFundamentalDto
{
    public int Id { get; set; }
    public string Simbolo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? ParticulaPortadora { get; set; }
}
