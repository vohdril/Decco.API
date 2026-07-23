using System;

namespace Decco.Api.DataLayer.Models;

public partial class NotificacaoAnomalia : IEntity
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string LocalIdentificado { get; set; } = null!;
    public DateTime? DataHora { get; set; }
    public string? Status { get; set; }
    public int NivelPrioridade { get; set; }
    public string? Relator { get; set; }
    public int? AnomaliaId { get; set; }
    public DateTime? DataResolucao { get; set; }

    public virtual Anomalium? Anomalium { get; set; }
}
