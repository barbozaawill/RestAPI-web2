using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeForum.Data;
using CodeForum.Models;

namespace CodeForum.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerguntaController : ControllerBase
{
    private readonly AppDbContext _context;

    public PerguntaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await _context.Perguntas.ToListAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var pergunta = await _context.Perguntas.FindAsync(id);
            if (pergunta == null)
                return NotFound(new { message = $"Pergunta {id} não encontrada." });

            return Ok(pergunta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Pergunta pergunta)
    {
        if (string.IsNullOrWhiteSpace(pergunta.Titulo))
            return BadRequest(new { message = "O campo 'Titulo' é obrigatório." });
        if (string.IsNullOrWhiteSpace(pergunta.Descricao))
            return BadRequest(new { message = "O campo 'Descricao' é obrigatória." });
        if (pergunta.UsuarioId <= 0)
            return BadRequest(new { message = "O campo 'UsuarioId' é obrigatório." });

        try
        {
            pergunta.DataCriacao = DateTime.UtcNow;
            _context.Perguntas.Add(pergunta);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = pergunta.Id }, pergunta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao salvar no banco de dados.", detail = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Pergunta pergunta)
    {
        if (string.IsNullOrWhiteSpace(pergunta.Titulo))
            return BadRequest(new { message = "O campo 'Titulo' é obrigatório." });
        if (string.IsNullOrWhiteSpace(pergunta.Descricao))
            return BadRequest(new { message = "O campo 'Descricao' é obrigatória." });

        try
        {
            var existing = await _context.Perguntas.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Pergunta {id} não encontrada." });

            existing.Titulo = pergunta.Titulo;
            existing.Descricao = pergunta.Descricao;
            await _context.SaveChangesAsync();
            return Ok(existing);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao atualizar no banco de dados.", detail = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var pergunta = await _context.Perguntas.FindAsync(id);
            if (pergunta == null)
                return NotFound(new { message = $"Pergunta {id} não encontrada." });

            _context.Perguntas.Remove(pergunta);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "Não é possível excluir esta pergunta pois ela possui respostas vinculadas.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }
}
