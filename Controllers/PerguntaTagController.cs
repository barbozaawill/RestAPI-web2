using CodeForum.Models;
using CodeForum.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeForum.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PerguntaTagController : ControllerBase
{
    private readonly IPerguntaTagRepository _repository;

    public PerguntaTagController(IPerguntaTagRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await _repository.GetAllAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }

    [HttpGet("{perguntaId}/{tagId}")]
    public async Task<IActionResult> GetById(int perguntaId, int tagId)
    {
        try
        {
            var perguntaTag = await _repository.GetByIdAsync(perguntaId, tagId);
            if (perguntaTag == null)
                return NotFound(new { message = $"Associação Pergunta {perguntaId} / Tag {tagId} não encontrada." });

            return Ok(perguntaTag);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PerguntaTag perguntaTag)
    {
        if (perguntaTag.PerguntaId <= 0)
            return BadRequest(new { message = "O campo 'PerguntaId' é obrigatório." });
        if (perguntaTag.TagId <= 0)
            return BadRequest(new { message = "O campo 'TagId' é obrigatório." });

        try
        {
            await _repository.AddAsync(perguntaTag);
            await _repository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById),
                new { perguntaId = perguntaTag.PerguntaId, tagId = perguntaTag.TagId },
                perguntaTag);
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "Essa associação já existe ou os IDs informados são inválidos.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao salvar no banco de dados.", detail = ex.Message });
        }
    }

    [HttpDelete("{perguntaId}/{tagId}")]
    public async Task<IActionResult> Delete(int perguntaId, int tagId)
    {
        try
        {
            var perguntaTag = await _repository.GetByIdAsync(perguntaId, tagId);
            if (perguntaTag == null)
                return NotFound(new { message = $"Associação Pergunta {perguntaId} / Tag {tagId} não encontrada." });

            await _repository.DeleteAsync(perguntaId, tagId);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "Erro de integridade ao deletar a associação.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }
}