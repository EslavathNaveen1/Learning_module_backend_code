using QtechBackend.Interfaces;
using QtechBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QtechBackend.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;

        public PlaylistService(IPlaylistRepository playlistRepository)
        {
            _playlistRepository = playlistRepository;
        }

        public async Task<IEnumerable<Playlist>> GetPlaylistsAsync()
        {
            return await _playlistRepository.GetPlaylistsAsync();
        }

        public async Task<Playlist> GetPlaylistByIdAsync(int id)
        {
            return await _playlistRepository.GetPlaylistByIdAsync(id);
        }

        public async Task<Playlist> AddPlaylistAsync(Playlist playlist)
        {
            return await _playlistRepository.AddPlaylistAsync(playlist);
        }

        public async Task<Playlist> UpdatePlaylistAsync(Playlist playlist)
        {
            return await _playlistRepository.UpdatePlaylistAsync(playlist);
        }

        public async Task<Playlist> PatchPlaylistAsync(int id, Dictionary<string, object> patchData)
        {
            return await _playlistRepository.PatchPlaylistAsync(id, patchData);
        }

        public async Task DeletePlaylistAsync(int id)
        {
            await _playlistRepository.DeletePlaylistAsync(id);
        }
    }
}