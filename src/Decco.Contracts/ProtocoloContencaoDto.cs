namespace Decco.Contracts;

public class ProtocoloContencaoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int NivelUrgencia { get; set; }
    public string? ClassesAplicaveis { get; set; }
    public string Passos { get; set; } = string.Empty;
    public string? RecursosNecessarios { get; set; }
    public DateTime? DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
