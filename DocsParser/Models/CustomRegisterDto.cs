

using System.ComponentModel.DataAnnotations;
namespace DocsParser.Models;
public class CustomRegisterDto
{
    [Required]
    [MaxLength(200)]
    public required string Email { get; set; }
    public required string Password { get; set; }
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public required string LastName { get; set; } = string.Empty;
}