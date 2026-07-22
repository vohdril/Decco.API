using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class Artefato
{
    public int Id { get; set; }

    public int AnomaliaId { get; set; }

    public string Identificacao { get; set; } = null!;

    public string? Material { get; set; }

    public DateOnly? DataFabricacao { get; set; }

    public string? LocalOrigem { get; set; }

    public string? PropriedadeSpin { get; set; }

    public decimal? PesoKg { get; set; }

    public string? Dimensoes { get; set; }

    public string? ModoUsar { get; set; }

    public virtual Anomalium Anomalia { get; set; } = null!;
}
