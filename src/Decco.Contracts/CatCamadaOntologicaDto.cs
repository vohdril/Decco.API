namespace Decco.Contracts;

public class CatCamadaOntologicaDto
{
    public int Id { get; set; }
    public string Simbolo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int? ForcaFundamentalId { get; set; }
    public int Prioridade { get; set; }
}
