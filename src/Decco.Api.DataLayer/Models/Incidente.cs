using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class Incidente : IEntity
{
    public int Id { get; set; }

    public int AnomaliaId { get; set; }

    public DateTime? DataHora { get; set; }

    public string Tipo { get; set; } = null!;

    public string Titulo { get; set; } = null!;

    public string Relatorio { get; set; } = null!;

    public string NivelSeguranca { get; set; } = null!;

    public bool? IsEventoSigma { get; set; }

    public int? Mortes { get; set; }

    public int? Feridos { get; set; }

    public string? DanoMaterial { get; set; }

    public virtual Anomalium Anomalia { get; set; } = null!;
}
