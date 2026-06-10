using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeForum.Data;
using CodeForum.Models;
using Microsoft.AspNetCore.Authorization;

namespace CodeForum.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuarioController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var usuarios = await _context.Usuarios
                .Select(u => new { u.Id, u.Nome, u.Login })
                .ToListAsync();
            return Ok(usuarios);
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
            var usuario = await _context.Usuarios
                .Where(u => u.Id == id)
                .Select(u => new { u.Id, u.Nome, u.Login })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound(new { message = $"Usuário {id} não encontrado." });

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario.Nome))
            return BadRequest(new { message = "O campo 'Nome' é obrigatório." });
        if (string.IsNullOrWhiteSpace(usuario.Login))
            return BadRequest(new { message = "O campo 'Login' é obrigatório." });
        if (string.IsNullOrWhiteSpace(usuario.Senha))
            return BadRequest(new { message = "O campo 'Senha' é obrigatória." });

        usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

        try
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, new { usuario.Id, usuario.Nome, usuario.Login });
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "Já existe um usuário com esse login.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao salvar no banco de dados.", detail = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario.Nome))
            return BadRequest(new { message = "O campo 'Nome' é obrigatório." });
        if (string.IsNullOrWhiteSpace(usuario.Login))
            return BadRequest(new { message = "O campo 'Login' é obrigatório." });

        try
        {
            var existing = await _context.Usuarios.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Usuário {id} não encontrado." });

            existing.Nome = usuario.Nome;
            existing.Login = usuario.Login;
            if (!string.IsNullOrWhiteSpace(usuario.Senha))
                existing.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

            await _context.SaveChangesAsync();
            return Ok(new { existing.Id, existing.Nome, existing.Login });
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "Já existe um usuário com esse login.", detail = ex.Message });
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
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { message = $"Usuário {id} não encontrado." });

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "Não é possível excluir este usuário pois ele possui registros vinculados.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }
}
