using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class Instalacao : IEntity
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string? Descricao { get; set; }
    public int TipoInstalacaoId { get; set; }
    public int? InstalacaoPaiId { get; set; }
    public string? Responsavel { get; set; }
    public string? Especialidade { get; set; }
    public int NivelAcessoMinimo { get; set; }
    public string Status { get; set; } = null!;
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
    public string UsuarioCriacao { get; set; } = null!;
    public string UsuarioAtualizacao { get; set; } = null!;

    public virtual CatTipoInstalacao TipoInstalacao { get; set; } = null!;

    public virtual Instalacao? InstalacaoPai { get; set; }

    public virtual ICollection<Instalacao> Filhas { get; set; } = new List<Instalacao>();
}
