using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class EntidadeViva : IEntity
{
    public int Id { get; set; }

    public int AnomaliaId { get; set; }

    public string Identificacao { get; set; } = null!;

    public string Especie { get; set; } = null!;

    public string? Biologia { get; set; }

    public string? OrigemPoder { get; set; }

    public DateOnly? DataNascimento { get; set; }

    public bool? IsConsciente { get; set; }

    public int? NivelInteligencia { get; set; }

    public string? Dieta { get; set; }

    public string? Observacoes { get; set; }

    public virtual Anomalium Anomalia { get; set; } = null!;
}
