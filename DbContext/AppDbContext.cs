using Microsoft.EntityFrameworkCore;
using CodeForum.Models;

namespace CodeForum.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Pergunta> Perguntas { get; set; }
    public DbSet<Resposta> Respostas { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<PerguntaTag> PerguntaTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Chave composta da tabela N-N
        modelBuilder.Entity<PerguntaTag>()
            .HasKey(pt => new { pt.PerguntaId, pt.TagId });

        modelBuilder.Entity<PerguntaTag>()
            .HasOne(pt => pt.Pergunta)
            .WithMany(p => p.PerguntaTags)
            .HasForeignKey(pt => pt.PerguntaId);

        modelBuilder.Entity<PerguntaTag>()
            .HasOne(pt => pt.Tag)
            .WithMany(t => t.PerguntaTags)
            .HasForeignKey(pt => pt.TagId);

        // Login único por usuário
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Login)
            .IsUnique();
    }
}