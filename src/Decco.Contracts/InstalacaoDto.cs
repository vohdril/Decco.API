namespace Decco.Contracts;

/// <summary>
/// Lugar físico do DeCCO — sítio, laboratório, área de contenção ou posto avançado.
/// Substitui o antigo LaboratorioDto: um laboratório é uma instalação do tipo
/// LABORATORIO cujo pai é um sítio.
/// </summary>
public class InstalacaoDto
{
    public int Id { get; set; }

    /// <summary>Identidade pública (ex.: SITIO-19, LAB-BIO-19). Não muda depois de criada.</summary>
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
