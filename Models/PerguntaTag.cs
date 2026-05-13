namespace CodeForum.Models;
public class PerguntaTag
{
    public int PerguntaId { get; set; }
    public Pergunta? Pergunta { get; set; }
    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}
