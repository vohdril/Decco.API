using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class CatCognicaoAparente : IEntity
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;

    public virtual ICollection<Anomalium> Anomalia { get; set; } = new List<Anomalium>();
}
