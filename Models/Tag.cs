namespace CodeForum.Models;
public class Tag
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public ICollection<PerguntaTag> PerguntaTags { get; set; } = new List<PerguntaTag>();
}
