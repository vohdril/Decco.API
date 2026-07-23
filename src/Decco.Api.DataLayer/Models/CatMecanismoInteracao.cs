using System;
using System.Collections.Generic;

namespace Decco.Api.DataLayer.Models;

public partial class CatMecanismoInteracao : IEntity
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public int CamadaOntologicaId { get; set; }

    public bool EhSubnatureza { get; set; }

    public virtual ICollection<Anomalium> AnomaliumMecanismoPrimarios { get; set; } = new List<Anomalium>();

    public virtual ICollection<Anomalium> AnomaliumMecanismoSecundarios { get; set; } = new List<Anomalium>();

    public virtual CatCamadaOntologica CamadaOntologica { get; set; } = null!;

    public virtual ICollection<PericiaAnomalium> PericiaAnomaliumMecanismoPrimarios { get; set; } = new List<PericiaAnomalium>();

    public virtual ICollection<PericiaAnomalium> PericiaAnomaliumMecanismoSecundarios { get; set; } = new List<PericiaAnomalium>();
}
