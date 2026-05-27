using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeForum.Data;
using CodeForum.Models;

namespace CodeForum.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RespostaController : ControllerBase
{
    private readonly AppDbContext _context;

    public RespostaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await _context.Respostas.ToListAsync());
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
            var resposta = await _context.Respostas.FindAsync(id);
            if (resposta == null)
                return NotFound(new { message = $"Resposta {id} não encontrada." });

            return Ok(resposta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Resposta resposta)
    {
        if (string.IsNullOrWhiteSpace(resposta.Conteudo))
            return BadRequest(new { message = "O campo 'Conteudo' é obrigatório." });
        if (resposta.PerguntaId <= 0)
            return BadRequest(new { message = "O campo 'PerguntaId' é obrigatório." });
        if (resposta.UsuarioId <= 0)
            return BadRequest(new { message = "O campo 'UsuarioId' é obrigatório." });

        try
        {
            resposta.DataCriacao = DateTime.UtcNow;
            _context.Respostas.Add(resposta);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = resposta.Id }, resposta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao salvar no banco de dados.", detail = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Resposta resposta)
    {
        if (string.IsNullOrWhiteSpace(resposta.Conteudo))
            return BadRequest(new { message = "O campo 'Conteudo' é obrigatório." });

        try
        {
            var existing = await _context.Respostas.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Resposta {id} não encontrada." });

            existing.Conteudo = resposta.Conteudo;
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
            var resposta = await _context.Respostas.FindAsync(id);
            if (resposta == null)
                return NotFound(new { message = $"Resposta {id} não encontrada." });

            _context.Respostas.Remove(resposta);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "Erro de integridade ao deletar a resposta.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }
}
