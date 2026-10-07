namespace Decco.Contracts;

/// <summary>
/// Investigation, research or suppression conducted INSIDE a facility.
/// The first facility-scoped entity: InstalacaoId is mandatory on creation and
/// never changes afterwards.
/// </summary>
public class OperacaoDto
{
    public int Id { get; set; }

    /// <summary>OP-{year}-{sequence}. When empty on Insert, the database generates it.</summary>
    public string? Codigo { get; set; }
    public string Codinome { get; set; } = string.Empty;

    public int TipoOperacaoId { get; set; }
    public string? TipoOperacaoCodigo { get; set; }
    public string? TipoOperacao { get; set; }

    public int InstalacaoId { get; set; }
    public string? InstalacaoCodigo { get; set; }
    public string? Instalacao { get; set; }

    public int? AnomaliaId { get; set; }
    public string? AnomaliaCodigo { get; set; }
    public int? NotificacaoId { get; set; }
    public int? ProtocoloId { get; set; }
    public string? ProtocoloCodigo { get; set; }

    public string Objetivo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Status { get; set; } = "PLANEJADA";
    public int Prioridade { get; set; } = 3;
    public int NivelAcessoMinimo { get; set; } = 1;
    public string? Responsavel { get; set; }

    public DateTime? DataAbertura { get; set; }
    public DateTime? DataPrevisaoTermino { get; set; }
    public DateTime? DataEncerramento { get; set; }
    public string? ResultadoResumo { get; set; }
    public DateTime? DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}

/// <summary>
/// Operation list filter — the body of POST /api/Operacao/List.
/// This is where the POST envelope pays off: the contract states what the query
/// slices (facility, type, status, clearance) without relying on a query string.
/// Property names mirror the sp_Operacao_Buscar parameters (DeccoDB vocabulary).
/// </summary>
public class OperacaoFilterDto : PageRequest
{
    public int? InstalacaoId { get; set; }

    /// <summary>Includes the operations of child facilities (e.g. the laboratories of a site).</summary>
    public bool IncluirSubinstalacoes { get; set; } = true;

    public int? TipoOperacaoId { get; set; }
    public string? Status { get; set; }
    public int? AnomaliaId { get; set; }

    /// <summary>Reader clearance: returns only operations whose NivelAcessoMinimo ≤ this value.</summary>
    public int? NivelAcessoUsuario { get; set; }
}
