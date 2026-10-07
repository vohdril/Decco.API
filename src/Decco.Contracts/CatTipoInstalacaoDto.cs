namespace Decco.Contracts;

public class CatTipoInstalacaoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool PermiteFilhos { get; set; }
    public bool Ativo { get; set; } = true;
}
