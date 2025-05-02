using QtechBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QtechBackend.Interfaces
{
    public interface IPlaylistRepository
    {
        Task<IEnumerable<Playlist>> GetPlaylistsAsync();
        Task<Playlist> GetPlaylistByIdAsync(int id);
        Task<Playlist> AddPlaylistAsync(Playlist playlist);
        Task<Playlist> UpdatePlaylistAsync(Playlist playlist);
        Task<Playlist> PatchPlaylistAsync(int id, Dictionary<string, object> patchData);
        Task DeletePlaylistAsync(int id);
    }
}