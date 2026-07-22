using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class CatForcaFundamental
{
    public int Id { get; set; }

    public string Simbolo { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public string? ParticulaPortadora { get; set; }

    public virtual ICollection<CatCamadaOntologica> CatCamadaOntologicas { get; set; } = new List<CatCamadaOntologica>();
}
