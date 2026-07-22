namespace Decco.Contracts;

public class CatClasseObjetoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string TipoClasse { get; set; } = string.Empty;
    public string? ClasseAcs { get; set; }
    public string? Descricao { get; set; }
    public int NivelAcessoMinimo { get; set; }
    public string? CorAlerta { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
}
