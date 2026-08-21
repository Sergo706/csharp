using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DocsParser.Models;

public class AppUser : IdentityUser
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(TypeName = "TIMESTAMP")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column(TypeName = "TIMESTAMP")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<Document> Documents { get; set; } = [];
}

public class Document
{
    public int Id { get; set; }
    [MaxLength(100)]
    public required string Title { get; set; }
    public required string UserId { get; set; }
    [MaxLength(50)]
    public required string ConvertedTo { get; set; }
    [MaxLength(50)]
    public required string ConvertedFrom { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(TypeName = "TIMESTAMP")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column(TypeName = "TIMESTAMP")]
    public DateTime UpdatedAt { get; set; }
    public AppUser? User { get; set; }
}


public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }
    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
            .Property(u => u.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();


        builder.Entity<Document>()
            .Property(d => d.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
    }
}