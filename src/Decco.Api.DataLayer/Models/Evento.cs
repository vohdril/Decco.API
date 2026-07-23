using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class Evento : IEntity
{
    public int Id { get; set; }

    public int AnomaliaId { get; set; }

    public string Nome { get; set; } = null!;

    public DateTime DataHoraInicio { get; set; }

    public DateTime? DataHoraFim { get; set; }

    public string? Periodicidade { get; set; }

    public string? ZonaAfetada { get; set; }

    public TimeOnly? DuracaoMedia { get; set; }

    public string? PreCondicoes { get; set; }

    public virtual Anomalium Anomalia { get; set; } = null!;
}
