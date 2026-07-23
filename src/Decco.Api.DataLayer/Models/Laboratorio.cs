using System;

namespace Decco.Api.DataLayer.Models;

public partial class Laboratorio : IEntity
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string? Descricao { get; set; }
    public string Sitio { get; set; } = null!;
    public string? Responsavel { get; set; }
    public string? Especialidade { get; set; }
    public int NivelAcessoMinimo { get; set; }
    public string? Status { get; set; }
    public DateTime? DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
