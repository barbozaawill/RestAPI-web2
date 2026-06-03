using Microsoft.EntityFrameworkCore;
using CodeForum.Data;
using CodeForum.Models;
using CodeForum.Repositories.Interfaces;

namespace CodeForum.Repositories;

public class PerguntaTagRepository : Repository<PerguntaTag>, IPerguntaTagRepository
{
    public PerguntaTagRepository(AppDbContext context) : base(context) { }

    public async Task<PerguntaTag?> GetByIdAsync(int perguntaId, int tagId) =>
        await _dbSet.FindAsync(perguntaId, tagId);

    public async Task DeleteAsync(int perguntaId, int tagId)
    {
        var entity = await GetByIdAsync(perguntaId, tagId);
        if (entity != null)
            _dbSet.Remove(entity);
    }
}