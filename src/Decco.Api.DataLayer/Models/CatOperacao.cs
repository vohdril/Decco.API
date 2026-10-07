using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class CatOperacao : IEntity
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public bool RequerAnomalia { get; set; }
    public int NivelAcessoMinimo { get; set; }
    public string? CorAlerta { get; set; }
    public bool Ativo { get; set; }

    public virtual ICollection<Operacao> Operacoes { get; set; } = new List<Operacao>();
}
