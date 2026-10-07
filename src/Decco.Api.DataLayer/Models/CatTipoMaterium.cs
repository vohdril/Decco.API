using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class CatTipoMaterium : IEntity
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public bool IsResistenteSupressores { get; set; }

    public virtual ICollection<Anomalia> Anomalia { get; set; } = new List<Anomalia>();
}
