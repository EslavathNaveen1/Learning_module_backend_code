using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QtechBackend.Context;
using QtechBackend.Interfaces;
using QtechBackend.Models;

public class DocumentationRepository : IDocumentationRepository
{
    private readonly ElearningContext _context;
    private readonly ILogger<DocumentationService> _logger;

    public DocumentationRepository(ElearningContext context, ILogger<DocumentationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Documentation> AddDocumentAsync(Documentation document)
    {
        await _context.Documentations.AddAsync(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task<Documentation> GetDocumentByIdAsync(int docId)
    {
        return await _context.Documentations
            .FirstOrDefaultAsync(d => d.DocId == docId);
    }

    public async Task<IEnumerable<Documentation>> GetDocumentsByPlaylistIdAsync(int playlistId)
    {
        return await _context.Documentations
            .Where(d => d.PlaylistId == playlistId)
            .ToListAsync();
    }

    public async Task UpdateDocumentAsync(Documentation document)
    {
        _context.Documentations.Update(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDocumentAsync(int docId)
    {
        var document = await GetDocumentByIdAsync(docId);
        if (document != null)
        {
            _context.Documentations.Remove(document);
            await _context.SaveChangesAsync();
        }
    }


    public async Task<IEnumerable<Documentation>> GetAllDocumentsAsync()
    {
        return await _context.Documentations
        .ToListAsync();
    }

    public async Task<Documentation> GetPdfByIdAsync(int id)
    {
        try
        {
            return await _context.Documentations
                .FirstOrDefaultAsync(d => d.DocId == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving PDF document with ID {id}");
            throw;
        }
    }

    public async Task<bool> UpdatePdfAsync(int id, byte[] fileContent, string fileName)
    {
        try
        {
            var document = await _context.Documentations
                .FirstOrDefaultAsync(d => d.DocId == id);

            if (document == null)
                return false;

            document.FileContent = fileContent;
            document.FileName = fileName;
            document.UpdatedAt = DateTime.UtcNow;

            _context.Documentations.Update(document);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"PDF document {id} updated successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating PDF document {id}");
            throw;
        }
    }










}