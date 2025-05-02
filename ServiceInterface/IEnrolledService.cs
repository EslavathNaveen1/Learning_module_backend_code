using System.Threading.Tasks;
using QtechBackendv2.Models;

namespace QtechBackendv2.ServiceInterface
{
    public interface IEnrolledService
    {
        Task<List<UserPlaylistDto>> GetUserPlaylistsAsync();
        Task<IEnumerable<UserPlaylistDto>> GetApprovedEnrollmentsAsync();
        Task<List<int>> GetPlaylistIdsByUserEmailAsync(string userEmail);
        Task<Enrolled> CreateEnrolledAsync(Enrolled enrolled);
        Task DeleteEnrolledAsync(string userEmail, int playlistId);
        Task<Enrolled> UpdateEnrolledAsync(Enrolled enrolled);

        Task ApproveAllEnrollmentsAsync();

        Task<List<UserPlaylistDto>> GetPendingEnrollmentsAsync();

    }
}
