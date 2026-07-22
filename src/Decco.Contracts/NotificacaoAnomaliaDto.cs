namespace Decco.Contracts;

public class NotificacaoAnomaliaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string LocalIdentificado { get; set; } = string.Empty;
    public DateTime? DataHora { get; set; }
    public string Status { get; set; } = "PENDENTE";
    public int NivelPrioridade { get; set; }
    public string? Relator { get; set; }
    public int? AnomaliaId { get; set; }
    public DateTime? DataResolucao { get; set; }
}
