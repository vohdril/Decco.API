using System;

namespace Decco.Api.DataLayer.Models;

/// <summary>
/// Row returned by sp_Operacao_Buscar — a READ MODEL, not an entity.
/// It does not implement IEntity and has no IEntityTypeConfiguration: EF does not know it;
/// Dapper materializes it, column by column, by name.
/// </summary>
public class OperacaoSummary
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

/// <summary>Parameters of sp_Operacao_Buscar. PageIndex is 0-based; the stored procedure is 1-based.</summary>
public record OperacaoFilter(
    int? InstalacaoId = null,
    bool IncluirSubinstalacoes = true,
    int? TipoOperacaoId = null,
    string? Status = null,
    int? AnomaliaId = null,
    int? NivelAcessoUsuario = null,
    int PageIndex = 0,
    int PageSize = 50);
