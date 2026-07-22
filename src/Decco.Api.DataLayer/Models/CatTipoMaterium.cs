using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class CatTipoMaterium
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public bool IsResistenteSupressores { get; set; }

    public virtual ICollection<Anomalium> Anomalia { get; set; } = new List<Anomalium>();
}
