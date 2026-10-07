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

    public virtual ICollection<Anomalia> AnomaliaMecanismoPrimarios { get; set; } = new List<Anomalia>();

    public virtual ICollection<Anomalia> AnomaliaMecanismoSecundarios { get; set; } = new List<Anomalia>();

    public virtual CatCamadaOntologica CamadaOntologica { get; set; } = null!;

    public virtual ICollection<PericiaAnomalia> PericiaAnomaliaMecanismoPrimarios { get; set; } = new List<PericiaAnomalia>();

    public virtual ICollection<PericiaAnomalia> PericiaAnomaliaMecanismoSecundarios { get; set; } = new List<PericiaAnomalia>();
}
