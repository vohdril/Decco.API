using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class CatCamadaOntologica : IEntity
{
    public int Id { get; set; }

    public string Simbolo { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public int? ForcaFundamentalId { get; set; }

    public int Prioridade { get; set; }

    public virtual ICollection<Anomalium> Anomalia { get; set; } = new List<Anomalium>();

    public virtual ICollection<CatMecanismoInteracao> CatMecanismoInteracaos { get; set; } = new List<CatMecanismoInteracao>();

    public virtual CatForcaFundamental? ForcaFundamental { get; set; }
}
