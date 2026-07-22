using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class Anomalium
{
    public int Id { get; set; }

    public string CodigoScp { get; set; } = null!;

    public string NomeComum { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public int ClasseObjetoId { get; set; }

    public int CamadaOntologicaId { get; set; }

    public int TipoMateriaId { get; set; }

    public int? CognicaoAparenteId { get; set; }

    public int? PericulosidadeId { get; set; }

    public int MecanismoPrimarioId { get; set; }

    public int? MecanismoSecundarioId { get; set; }

    public decimal? IeiaDBase { get; set; }

    public string? FatorCoerenciaSpin { get; set; }

    public string? Status { get; set; }

    public string? SitioContencao { get; set; }

    public string? ResponsavelPesquisa { get; set; }

    public DateTime? DataCriacao { get; set; }

    public DateTime? DataAtualizacao { get; set; }

    public string? UsuarioCriacao { get; set; }

    public string? UsuarioAtualizacao { get; set; }

    public virtual ICollection<Artefato> Artefatos { get; set; } = new List<Artefato>();

    public virtual CatCamadaOntologica CamadaOntologica { get; set; } = null!;

    public virtual CatClasseObjeto ClasseObjeto { get; set; } = null!;

    public virtual ICollection<EntidadeViva> EntidadeVivas { get; set; } = new List<EntidadeViva>();

    public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();

    public virtual CatCognicaoAparente? CognicaoAparente { get; set; }

    public virtual CatPericulosidade? Periculosidade { get; set; }

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();

    public virtual ICollection<Localidade> Localidades { get; set; } = new List<Localidade>();

    public virtual ICollection<NotificacaoAnomalia> NotificacaoAnomalias { get; set; } = new List<NotificacaoAnomalia>();

    public virtual ICollection<ProtocoloAplicadoEm> ProtocoloAplicadoEms { get; set; } = new List<ProtocoloAplicadoEm>();

    public virtual CatMecanismoInteracao MecanismoPrimario { get; set; } = null!;

    public virtual CatMecanismoInteracao? MecanismoSecundario { get; set; }

    public virtual ICollection<PericiaAnomalium> PericiaAnomalia { get; set; } = new List<PericiaAnomalium>();

    public virtual CatTipoMaterium TipoMateria { get; set; } = null!;
}
