using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class CatClasseObjeto : IEntity
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string TipoClasse { get; set; } = null!;

    public string? ClasseAcs { get; set; }

    public string? Descricao { get; set; }

    public int NivelAcessoMinimo { get; set; }

    public string? CorAlerta { get; set; }

    public DateTime DataCriacao { get; set; }

    public bool Ativo { get; set; }

    public virtual ICollection<Anomalium> Anomalia { get; set; } = new List<Anomalium>();
}
