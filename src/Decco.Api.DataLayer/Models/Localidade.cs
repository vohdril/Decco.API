using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class Localidade
{
    public int Id { get; set; }

    public int AnomaliaId { get; set; }

    public string Nome { get; set; } = null!;

    public int? RaioEfeitoMetros { get; set; }

    public decimal? IeiaDAmbiente { get; set; }

    public bool? IsGeograficamenteLimitada { get; set; }

    public string? TipoTerreno { get; set; }

    public string? ClimaAnomalo { get; set; }

    public virtual Anomalium Anomalia { get; set; } = null!;
}
