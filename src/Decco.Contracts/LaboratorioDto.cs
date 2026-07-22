namespace Decco.Contracts;

public class LaboratorioDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Sitio { get; set; } = string.Empty;
    public string? Responsavel { get; set; }
    public string? Especialidade { get; set; }
    public int NivelAcessoMinimo { get; set; }
    public string Status { get; set; } = "ATIVO";
    public DateTime? DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
