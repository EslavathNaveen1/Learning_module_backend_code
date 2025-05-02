using QtechBackend.Models;
using QtechBackend.RepositoryInterface;
using QtechBackend.ServiceInterface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QtechBackend.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;

        public VideoService(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task<IEnumerable<Video>> GetVideosAsync()
        {
            return await _videoRepository.GetVideosAsync();
        }

        public async Task<Video> GetVideoByIdAsync(int id)
        {
            return await _videoRepository.GetVideoByIdAsync(id);
        }

        public async Task<Video> AddVideoAsync(Video video)
        {
            return await _videoRepository.AddVideoAsync(video);
        }

        public async Task<Video> UpdateVideoAsync(Video video)
        {
            return await _videoRepository.UpdateVideoAsync(video);
        }

        public async Task<Video> PatchVideoAsync(int id, Dictionary<string, object> patchData)
        {
            return await _videoRepository.PatchVideoAsync(id, patchData);
        }

        public async Task DeleteVideoAsync(int id)
        {
            await _videoRepository.DeleteVideoAsync(id);
        }
    }
}