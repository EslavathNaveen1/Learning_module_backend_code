using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QtechBackend.Context;
using QtechBackend.Interfaces;
using QtechBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QtechBackend.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ElearningContext _context;

        public PlaylistRepository(ElearningContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Playlist>> GetPlaylistsAsync()
        {
            return await _context.Playlists.ToListAsync();
        }

        public async Task<Playlist> GetPlaylistByIdAsync(int id)
        {
            return await _context.Playlists.FindAsync(id);
        }

        public async Task<Playlist> AddPlaylistAsync(Playlist playlist)
        {
            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }

        public async Task<Playlist> UpdatePlaylistAsync(Playlist playlist)
        {
            _context.Playlists.Update(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }

        public async Task<Playlist> PatchPlaylistAsync(int id, Dictionary<string, object> patchData)
        {
            var existingPlaylist = await _context.Playlists.FindAsync(id);
            if (existingPlaylist == null) return null;

            foreach (var property in patchData)
            {
                var prop = existingPlaylist.GetType().GetProperty(property.Key);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(existingPlaylist, property.Value);
                }
            }
            await _context.SaveChangesAsync();
            return existingPlaylist;
        }

        public async Task DeletePlaylistAsync(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync();
            }
        }

    
    }
}