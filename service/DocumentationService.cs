using AutoMapper;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using QtechBackend.Controllers;
using QtechBackend.Interfaces;
using QtechBackend.Models;

public class DocumentationService : IDocumentationService
{
    private readonly IDocumentationRepository _documentRepository;
    private readonly ILogger<DocumentationService> _logger;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _environment;


    public DocumentationService(IDocumentationRepository documentRepository, ILogger<DocumentationService> logger, IMapper mapper
        , IWebHostEnvironment environment)
    {
        _documentRepository = documentRepository;
        _logger = logger;
        _mapper = mapper;
        _environment = environment;
    }

    public async Task SaveDocumentAsync(Documentation documentDto)
    {
        try
        {
            var documentation = new Documentation
            {
                FileName = documentDto.FileName,
                Content = documentDto.Content,
                FileContent = documentDto.FileContent,
                PlaylistId = documentDto.PlaylistId,
                CreatedAt = documentDto.CreatedAt,
                UpdatedAt = documentDto.UpdatedAt
            };

            await _documentRepository.AddDocumentAsync(documentation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving document");
            throw;
        }
    }

    public async Task<IEnumerable<Documentation>> GetDocumentsByPlaylistIdAsync(int playlistId)
    {
        try
        {
            return await _documentRepository.GetDocumentsByPlaylistIdAsync(playlistId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving documents for playlist");
            throw;
        }
    }

    public async Task DeleteDocumentAsync(int docId)
    {
        try
        {
            await _documentRepository.DeleteDocumentAsync(docId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document");
            throw;
        }
    }



    public async Task<IEnumerable<Documentation>> GetAllDocumentsAsync()
    {
        var documents = await _documentRepository.GetAllDocumentsAsync();
        return _mapper.Map<IEnumerable<Documentation>>(documents);
    }




    public async Task<Documentation> GetPdfByIdAsync(int id)
    {
        return await _documentRepository.GetPdfByIdAsync(id);
    }

    public async Task UpdatePdfAsync(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file uploaded.");

       
        byte[] fileContent;
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            fileContent = memoryStream.ToArray();
        }

      
        string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "pdfs");
        Directory.CreateDirectory(uploadFolder);
        string uniqueFileName = $"{id}_{Guid.NewGuid()}_{file.FileName}";
        string filePath = Path.Combine(uploadFolder, uniqueFileName);

        try
        {
            await File.WriteAllBytesAsync(filePath, fileContent);

            bool updateResult = await _documentRepository.UpdatePdfAsync(
                id,
                fileContent,
                uniqueFileName
            );

            if (!updateResult)
                throw new Exception("Failed to update PDF in database");

            _logger.LogInformation($"PDF {id} updated successfully. File saved to {filePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating PDF {id}");

            if (File.Exists(filePath))
                File.Delete(filePath);

            throw;
        }
    }












}
