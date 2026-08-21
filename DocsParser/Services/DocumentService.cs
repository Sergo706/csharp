using DocsParser.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace DocsParser.Services;
public class DocumentService
{
    private readonly AppDbContext _context;

    public DocumentService(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
    }

    public async Task AddDocumentHistory(string title, string userId, string convertedTo, string convertedFrom)
    {
        try
        {
            var Document = new Document
            {
                Title = title,
                UserId = userId,
                ConvertedFrom = convertedFrom,
                ConvertedTo = convertedTo
            };
            _context.Documents.Add(Document);
            await _context.SaveChangesAsync();
        } catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save document history.", ex);
        }
    }

    public async Task<AppUser?> GetAllDocumentHistory(string userId)
    {
        try
        {
            var docs = await _context.Users.Include(u => u.Documents).FirstOrDefaultAsync(u => u.Id == userId);
            return docs;
        } catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get all user's documents history", ex);
        }
    }
}