using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeForum.Data;
using CodeForum.Models;

namespace CodeForum.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly AppDbContext _context;

    public TagController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await _context.Tags.ToListAsync());
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
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
                return NotFound(new { message = $"Tag {id} não encontrada." });

            return Ok(tag);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Tag tag)
    {
        if (string.IsNullOrWhiteSpace(tag.Nome))
            return BadRequest(new { message = "O campo 'Nome' é obrigatório." });

        try
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao salvar no banco de dados.", detail = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Tag tag)
    {
        if (string.IsNullOrWhiteSpace(tag.Nome))
            return BadRequest(new { message = "O campo 'Nome' é obrigatório." });

        try
        {
            var existing = await _context.Tags.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Tag {id} não encontrada." });

            existing.Nome = tag.Nome;
            existing.Descricao = tag.Descricao;
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
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
                return NotFound(new { message = $"Tag {id} não encontrada." });

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "Não é possível excluir esta tag pois ela está vinculada a perguntas.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }
}
