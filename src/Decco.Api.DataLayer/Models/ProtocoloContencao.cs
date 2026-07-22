using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class ProtocoloContencao
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Titulo { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int NivelUrgencia { get; set; }
    public string? ClassesAplicaveis { get; set; }
    public string Passos { get; set; } = null!;
    public string? RecursosNecessarios { get; set; }
    public DateTime? DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    public virtual ICollection<ProtocoloAplicadoEm> ProtocoloAplicadoEms { get; set; } = new List<ProtocoloAplicadoEm>();
}
