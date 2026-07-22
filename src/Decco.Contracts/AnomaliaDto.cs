namespace Decco.Contracts;

public class AnomaliaDto
{
    public int Id { get; set; }
    public string CodigoSCP { get; set; } = string.Empty;
    public string NomeComum { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string ClasseObjeto { get; set; } = string.Empty;
    public string CamadaOntologica { get; set; } = string.Empty;
    public string TipoMateria { get; set; } = string.Empty;
    public string? CognicaoAparente { get; set; }
    public string? Periculosidade { get; set; }
    public string MecanismoPrimario { get; set; } = string.Empty;
    public string? MecanismoSecundario { get; set; }
    public decimal? IEIA_D_Base { get; set; }
    public string? FatorCoerenciaSpin { get; set; }
    public string Status { get; set; } = "ATIVA";
    public string? SitioContencao { get; set; }
    public string? ResponsavelPesquisa { get; set; }
    public DateTime? DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
