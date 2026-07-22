using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class PericiaAnomalium
{
    public int Id { get; set; }

    public int AnomaliaId { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public int MecanismoPrimarioId { get; set; }

    public int? MecanismoSecundarioId { get; set; }

    public int? Nivel { get; set; }

    public string? Custo { get; set; }

    public virtual Anomalium Anomalia { get; set; } = null!;

    public virtual ICollection<InstanciaPericiaDesviante> InstanciaPericiaDesviantes { get; set; } = new List<InstanciaPericiaDesviante>();

    public virtual CatMecanismoInteracao MecanismoPrimario { get; set; } = null!;

    public virtual CatMecanismoInteracao? MecanismoSecundario { get; set; }

    public virtual ICollection<PericiaManifestacao> PericiaManifestacaos { get; set; } = new List<PericiaManifestacao>();
}
