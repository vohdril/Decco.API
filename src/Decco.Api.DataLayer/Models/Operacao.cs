using System;

namespace Decco.Api.DataLayer.Models;

public partial class Operacao : IEntity
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Codinome { get; set; } = null!;
    public int TipoOperacaoId { get; set; }
    public int InstalacaoId { get; set; }
    public int? AnomaliaId { get; set; }
    public int? NotificacaoId { get; set; }
    public int? ProtocoloId { get; set; }
    public string Objetivo { get; set; } = null!;
    public string? Descricao { get; set; }
    public string Status { get; set; } = null!;
    public int Prioridade { get; set; }
    public int NivelAcessoMinimo { get; set; }
    public string? Responsavel { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime? DataPrevisaoTermino { get; set; }
    public DateTime? DataEncerramento { get; set; }
    public string? ResultadoResumo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
    public string UsuarioCriacao { get; set; } = null!;
    public string UsuarioAtualizacao { get; set; } = null!;

    public virtual CatOperacao TipoOperacao { get; set; } = null!;

    public virtual Instalacao Instalacao { get; set; } = null!;

    public virtual Anomalia? Anomalia { get; set; }

    public virtual NotificacaoAnomalia? Notificacao { get; set; }

    public virtual ProtocoloContencao? Protocolo { get; set; }
}
