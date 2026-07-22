using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class VwEstatisticasAnomalias
{
    public int? TotalAnomalias { get; set; }

    public int? Ativas { get; set; }

    public int? Neutralizadas { get; set; }

    public int? ClasseSafe { get; set; }

    public int? ClasseEuclid { get; set; }

    public int? ClasseKeter { get; set; }

    public int? ClasseThaumiel { get; set; }

    public int? ClasseApotheosis { get; set; }

    public int? CamadaTheta { get; set; }

    public int? CamadaPsi { get; set; }

    public int? CamadaPhi { get; set; }

    public int? CamadaOmega { get; set; }

    public int? MateriaBarionica { get; set; }

    public int? MateriaNaoBarionica { get; set; }

    public int? MateriaMista { get; set; }

    public decimal? IeiaDMedio { get; set; }

    public int? ResistenteSupressao { get; set; }
}
