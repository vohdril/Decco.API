using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class VwRelatorioSigma
{
    public string CodigoScp { get; set; } = null!;

    public string NomeComum { get; set; } = null!;

    public string Classe { get; set; } = null!;

    public string Camada { get; set; } = null!;

    public bool IsResistenteSupressores { get; set; }

    public int? TotalIncidentes { get; set; }

    public int? IncidentesSigma { get; set; }

    public DateTime? UltimoIncidente { get; set; }

    public string? SitioContencao { get; set; }

    public string? ResponsavelPesquisa { get; set; }
}
