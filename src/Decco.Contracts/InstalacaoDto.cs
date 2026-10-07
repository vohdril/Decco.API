namespace Decco.Contracts;

/// <summary>
/// A physical DeCCO facility — site, laboratory, containment area or forward post.
/// Replaces the former LaboratorioDto: a laboratory is a facility of type
/// LABORATORIO whose parent is a site.
/// </summary>
public class InstalacaoDto
{
    public int Id { get; set; }

    /// <summary>Public identity (e.g. SITIO-19, LAB-BIO-19). Never changes after creation.</summary>
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    public int TipoInstalacaoId { get; set; }
    public string? TipoInstalacaoCodigo { get; set; }
    public string? TipoInstalacao { get; set; }

    public int? InstalacaoPaiId { get; set; }
    public string? InstalacaoPaiCodigo { get; set; }

    public string? Responsavel { get; set; }
    public string? Especialidade { get; set; }
    public int NivelAcessoMinimo { get; set; } = 1;
    public string Status { get; set; } = "ATIVA";
    public DateTime? DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
