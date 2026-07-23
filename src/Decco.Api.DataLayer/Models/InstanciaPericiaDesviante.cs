using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class InstanciaPericiaDesviante : IEntity
{
    public int Id { get; set; }

    public string TipoInstancia { get; set; } = null!;

    public int InstanciaId { get; set; }

    public int PericiaDesvianteId { get; set; }

    public DateOnly? DataDescoberta { get; set; }

    public string? Intensidade { get; set; }

    public string? Observacoes { get; set; }

    public virtual PericiaAnomalium PericiaDesviante { get; set; } = null!;
}
