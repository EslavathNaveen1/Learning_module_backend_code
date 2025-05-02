using Microsoft.EntityFrameworkCore;
using QtechBackend.Context;
using QtechBackend.Models;
using QtechBackend.RepositoryInterface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QtechBackend.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly ElearningContext _context;

        public VideoRepository(ElearningContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Video>> GetVideosAsync()
        {
            return await _context.Videos.ToListAsync();
        }

        public async Task<Video> GetVideoByIdAsync(int id)
        {
            return await _context.Videos.FindAsync(id);
        }

        public async Task<Video> AddVideoAsync(Video video)
        {
            _context.Videos.Add(video);
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task<Video> UpdateVideoAsync(Video video)
        {
            _context.Entry(video).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task<Video> PatchVideoAsync(int id, Dictionary<string, object> patchData)
        {
            var existingVideo = await _context.Videos.FindAsync(id);
            if (existingVideo == null) return null;

            foreach (var property in patchData)
            {
                var prop = existingVideo.GetType().GetProperty(property.Key);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(existingVideo, property.Value);
                }
            }
            await _context.SaveChangesAsync();
            return existingVideo;
        }

        public async Task DeleteVideoAsync(int id)
        {
            var video = await _context.Videos.FindAsync(id);
            if (video != null)
            {
                _context.Videos.Remove(video);
                await _context.SaveChangesAsync();
            }
        }
    }
}