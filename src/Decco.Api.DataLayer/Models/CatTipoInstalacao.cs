using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class CatTipoInstalacao : IEntity
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public bool PermiteFilhos { get; set; }
    public bool Ativo { get; set; }

    public virtual ICollection<Instalacao> Instalacoes { get; set; } = new List<Instalacao>();
}
