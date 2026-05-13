namespace CodeForum.Models;

public class Resposta
{
    public int Id { get; set; }
    public string Conteudo { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public int PerguntaId { get; set; }
    public Pergunta? Pergunta { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}