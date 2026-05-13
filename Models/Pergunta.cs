namespace CodeForum.Models;

public class Pergunta
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public ICollection<Resposta> Respostas { get; set; } = new List<Resposta>();
    public ICollection<PerguntaTag> PerguntaTags { get; set; } = new List<PerguntaTag>();
}