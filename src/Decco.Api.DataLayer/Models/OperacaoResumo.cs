using System;

namespace Decco.Api.DataLayer.Models;

/// <summary>
/// Linha devolvida por sp_Operacao_Buscar — um READ MODEL, não uma entidade.
/// Não implementa IEntity e não tem IEntityTypeConfiguration: o EF não o
/// conhece; quem o materializa é o Dapper, coluna a coluna, pelo nome.
/// </summary>
public class OperacaoResumo
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Codinome { get; set; } = null!;
    public int TipoOperacaoId { get; set; }
    public string TipoOperacaoCodigo { get; set; } = null!;
    public string TipoOperacao { get; set; } = null!;
    public int InstalacaoId { get; set; }
    public string InstalacaoCodigo { get; set; } = null!;
    public string Instalacao { get; set; } = null!;
    public int? AnomaliaId { get; set; }
    public string? AnomaliaCodigo { get; set; }
    public int? NotificacaoId { get; set; }
    public int? ProtocoloId { get; set; }
    public string? ProtocoloCodigo { get; set; }
    public string Objetivo { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int Prioridade { get; set; }
    public int NivelAcessoMinimo { get; set; }
    public string? Responsavel { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime? DataPrevisaoTermino { get; set; }
    public DateTime? DataEncerramento { get; set; }
}

/// <summary>Parâmetros de sp_Operacao_Buscar. PageIndex é base 0; a SP é base 1.</summary>
public record OperacaoFiltro(
    int? InstalacaoId = null,
    bool IncluirSubinstalacoes = true,
    int? TipoOperacaoId = null,
    string? Status = null,
    int? AnomaliaId = null,
    int? NivelAcessoUsuario = null,
    int PageIndex = 0,
    int PageSize = 50);
