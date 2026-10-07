namespace Decco.Contracts;

public class CatOperacaoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool RequerAnomalia { get; set; }
    public int NivelAcessoMinimo { get; set; } = 1;
    public string? CorAlerta { get; set; }
    public bool Ativo { get; set; } = true;
}
