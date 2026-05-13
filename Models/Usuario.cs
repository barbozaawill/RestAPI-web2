namespace CodeForum.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;

    public ICollection<Pergunta> Perguntas { get; set; } = new List<Pergunta>();
    public ICollection<Resposta> Respostas { get; set; } = new List<Resposta>();
}
