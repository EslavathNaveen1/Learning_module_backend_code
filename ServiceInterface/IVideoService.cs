using QtechBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QtechBackend.ServiceInterface
{
    public interface IVideoService
    {
        Task<IEnumerable<Video>> GetVideosAsync();
        Task<Video> GetVideoByIdAsync(int id);
        Task<Video> AddVideoAsync(Video video);
        Task<Video> UpdateVideoAsync(Video video);
        Task<Video> PatchVideoAsync(int id, Dictionary<string, object> patchData);
        Task DeleteVideoAsync(int id);
    }
}