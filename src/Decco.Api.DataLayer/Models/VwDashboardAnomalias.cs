using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class VwDashboardAnomalias
{
    public int Id { get; set; }

    public string CodigoScp { get; set; } = null!;

    public string NomeComum { get; set; } = null!;

    public string ClasseObjeto { get; set; } = null!;

    public string? CorAlerta { get; set; }

    public string Camada { get; set; } = null!;

    public string CamadaOntologica { get; set; } = null!;

    public string TipoMateria { get; set; } = null!;

    public string MecanismoPrimario { get; set; } = null!;

    public string? MecanismoSecundario { get; set; }

    public decimal? IeiaDBase { get; set; }

    public string? FatorCoerenciaSpin { get; set; }

    public string? Status { get; set; }

    public string? SitioContencao { get; set; }

    public int NivelRisco { get; set; }

    public string StatusTheta { get; set; } = null!;

    public int? ContagemSigma { get; set; }

    public int? QtdEntidades { get; set; }

    public int? QtdArtefatos { get; set; }
}
