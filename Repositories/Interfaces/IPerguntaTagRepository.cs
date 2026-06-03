using CodeForum.Models;

namespace CodeForum.Repositories.Interfaces;

public interface IPerguntaTagRepository : IRepository<PerguntaTag>
{
    Task<PerguntaTag?> GetByIdAsync(int perguntaId, int tagId);
    Task DeleteAsync(int perguntaId, int tagId);
}