using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class PericiaManifestacao
{
    public int PericiaAnomaliaId { get; set; }

    public int ManifestacaoEspecificaId { get; set; }

    public string? Intensidade { get; set; }

    public string? Observacoes { get; set; }

    public virtual CatManifestacaoEspecifica ManifestacaoEspecifica { get; set; } = null!;

    public virtual PericiaAnomalium PericiaAnomalia { get; set; } = null!;
}
