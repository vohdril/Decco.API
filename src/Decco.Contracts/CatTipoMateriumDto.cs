namespace Decco.Contracts;

public class CatTipoMateriumDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool IsResistenteSupressores { get; set; }
}
