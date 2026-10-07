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

    // Known facility where the report was made — optional: LocalIdentificado remains
    // the textual description, and a report from outside the perimeter has none.
    public int? InstalacaoId { get; set; }
    public string? InstalacaoCodigo { get; set; }

    public DateTime? DataResolucao { get; set; }
}
