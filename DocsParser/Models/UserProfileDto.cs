namespace DocsParser.Models;

public record UserProfileDto(
    string Name,
    string? LastName,
    string? AvatarUrl,
    DateTime CreatedAt
);
