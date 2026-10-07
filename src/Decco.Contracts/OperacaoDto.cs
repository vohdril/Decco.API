namespace Decco.Contracts;

/// <summary>
/// Investigação, pesquisa ou supressão conduzida DENTRO de uma instalação.
/// É a primeira entidade escopada por instalação: InstalacaoId é obrigatório na
/// criação e não muda depois.
/// </summary>
public class OperacaoDto
{
    public int Id { get; set; }

    /// <summary>OP-{ano}-{sequencial}. Se vier vazio no Insert, o banco gera.</summary>
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
/// Filtro da listagem de operações — é o corpo do POST /api/Operacao/List.
/// É aqui que o envelope POST se justifica: o contrato diz o que a consulta
/// recorta (instalação, tipo, status, clearance) sem depender de query string.
/// </summary>
public class OperacaoFiltroDto : PageRequest
{
    public int? InstalacaoId { get; set; }

    /// <summary>Inclui as operações das instalações filhas (ex.: os laboratórios de um sítio).</summary>
    public bool IncluirSubinstalacoes { get; set; } = true;

    public int? TipoOperacaoId { get; set; }
    public string? Status { get; set; }
    public int? AnomaliaId { get; set; }

    /// <summary>Clearance do leitor: devolve só operações com NivelAcessoMinimo ≤ este valor.</summary>
    public int? NivelAcessoUsuario { get; set; }
}
