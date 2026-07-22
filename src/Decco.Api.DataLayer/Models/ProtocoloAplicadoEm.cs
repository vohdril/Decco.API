using System;

namespace Decco.Api.DataLayer.Models;

public partial class ProtocoloAplicadoEm
{
    public int ProtocoloId { get; set; }
    public int AnomaliaId { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Status { get; set; }
    public string? Observacoes { get; set; }

    public virtual Anomalium Anomalium { get; set; } = null!;
    public virtual ProtocoloContencao ProtocoloContencao { get; set; } = null!;
}
