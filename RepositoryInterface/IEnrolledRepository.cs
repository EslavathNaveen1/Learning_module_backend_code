using QtechBackendv2.Models;

namespace QtechBackendv2.RepositoryInterface
{
    public interface IEnrolledRepository
    {
        Task<List<UserPlaylistDto>> GetUserPlaylistsAsync();

        Task<List<int>> GetPlaylistIdsByUserEmailAsync(string userEmail);
        Task<Enrolled> CreateEnrolledAsync(Enrolled enrolled);
        Task DeleteEnrolledAsync(string userEmail, int playlistId);
        Task<Enrolled> UpdateEnrolledAsync(Enrolled enrolled);

        Task ApproveAllEnrollmentsAsync();

      
        Task<IEnumerable<UserPlaylistDto>> GetApprovedEnrollmentsAsync();

        Task<List<UserPlaylistDto>> GetPendingEnrollmentsAsync();

    }
}
