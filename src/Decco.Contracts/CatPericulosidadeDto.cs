namespace Decco.Contracts;

public class CatPericulosidadeDto
{
    public int Id { get; set; }
    public int Nivel { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? CorAlerta { get; set; }
}
