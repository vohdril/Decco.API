using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class CatPericulosidade : IEntity
{
    public int Id { get; set; }
    public int Nivel { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string? CorAlerta { get; set; }

    public virtual ICollection<Anomalium> Anomalia { get; set; } = new List<Anomalium>();
}
