namespace Decco.Contracts;

public class CatMecanismoInteracaoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int CamadaOntologicaId { get; set; }
    public bool EhSubnatureza { get; set; }
}
