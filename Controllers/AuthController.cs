using CodeForum.Data;
using CodeForum.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CodeForum.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public record LoginRequest(string Login, string Senha);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Senha))
            return BadRequest(new { message = "Login e senha são obrigatórios." });

        try
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Login == request.Login);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
                return Unauthorized(new { message = "Login ou senha inválidos." });

            var token = _tokenService.GenerateToken(usuario);

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao acessar o banco de dados.", detail = ex.Message });
        }
    }
}