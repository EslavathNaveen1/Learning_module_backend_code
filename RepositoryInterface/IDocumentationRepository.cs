using QtechBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QtechBackend.Interfaces
{
    public interface IDocumentationRepository
    {
        Task<Documentation> AddDocumentAsync(Documentation document);
        Task<Documentation> GetPdfByIdAsync(int id);
        Task<Documentation> GetDocumentByIdAsync(int docId);
        Task<IEnumerable<Documentation>> GetDocumentsByPlaylistIdAsync(int playlistId);
        Task<bool> UpdatePdfAsync(int id, byte[] fileContent, string fileName);
        Task DeleteDocumentAsync(int docId);
        Task<IEnumerable<Documentation>> GetAllDocumentsAsync();

    }
}