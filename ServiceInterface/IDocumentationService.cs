using QtechBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QtechBackend.Interfaces
{
    public interface IDocumentationService
    {
        Task SaveDocumentAsync(Documentation documentDto);
        Task<IEnumerable<Documentation>> GetDocumentsByPlaylistIdAsync(int playlistId);
        Task DeleteDocumentAsync(int docId);
        Task<IEnumerable<Documentation>> GetAllDocumentsAsync();
        Task UpdatePdfAsync(int id, IFormFile file);
        Task<Documentation> GetPdfByIdAsync(int id);
    }
}