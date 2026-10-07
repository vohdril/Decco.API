namespace Decco.Contracts;

public class AnomaliaDto
{
    public int Id { get; set; }
    public string CodigoSCP { get; set; } = string.Empty;
    public string NomeComum { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    // Classification — the Id is what gets WRITTEN; the name is read-only (filled on Get/List).
    public int ClasseObjetoId { get; set; }
    public string ClasseObjeto { get; set; } = string.Empty;
    public int CamadaOntologicaId { get; set; }
    public string CamadaOntologica { get; set; } = string.Empty;
    public int TipoMateriaId { get; set; }
    public string TipoMateria { get; set; } = string.Empty;
    public int? CognicaoAparenteId { get; set; }
    public string? CognicaoAparente { get; set; }
    public int? PericulosidadeId { get; set; }
    public string? Periculosidade { get; set; }
    public int MecanismoPrimarioId { get; set; }
    public string MecanismoPrimario { get; set; } = string.Empty;
    public int? MecanismoSecundarioId { get; set; }
    public string? MecanismoSecundario { get; set; }

    public decimal? IEIA_D_Base { get; set; }
    public string? FatorCoerenciaSpin { get; set; }
    public string Status { get; set; } = "ATIVA";

    // Where the anomaly is contained — replaces the former free-text SitioContencao.
    public int? InstalacaoContencaoId { get; set; }
    public string? InstalacaoContencaoCodigo { get; set; }
    public string? InstalacaoContencao { get; set; }

    public string? ResponsavelPesquisa { get; set; }
    public DateTime? DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
