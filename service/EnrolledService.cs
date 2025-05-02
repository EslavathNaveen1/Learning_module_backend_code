using QtechBackendv2.Models;
using QtechBackendv2.RepositoryInterface;
using QtechBackendv2.ServiceInterface;

public class EnrolledService : IEnrolledService
{
    private readonly IEnrolledRepository _enrolledRepository;

    public EnrolledService(IEnrolledRepository enrolledRepository)
    {
        _enrolledRepository = enrolledRepository;
    }

    public async Task<List<UserPlaylistDto>> GetUserPlaylistsAsync()
    {
        return await _enrolledRepository.GetUserPlaylistsAsync();
    }

    public async Task<List<int>> GetPlaylistIdsByUserEmailAsync(string userEmail)
    {
        return await _enrolledRepository.GetPlaylistIdsByUserEmailAsync(userEmail);
    }

    public async Task<IEnumerable<UserPlaylistDto>> GetApprovedEnrollmentsAsync()
    {
        return await _enrolledRepository.GetApprovedEnrollmentsAsync();
    }

    public async Task DeleteEnrolledAsync(string userEmail, int playlistId)
    {
        await _enrolledRepository.DeleteEnrolledAsync(userEmail, playlistId);
    }

    public  Task<Enrolled> UpdateEnrolledAsync(Enrolled enrolled)
    {
        return  _enrolledRepository.UpdateEnrolledAsync(enrolled);
    }

  
    public  Task<Enrolled> CreateEnrolledAsync(Enrolled enrolled)
    {
        return  _enrolledRepository.CreateEnrolledAsync(enrolled);
    }


    public async Task ApproveAllEnrollmentsAsync()
    {
        await _enrolledRepository.ApproveAllEnrollmentsAsync();
    }


    public async Task<List<UserPlaylistDto>> GetPendingEnrollmentsAsync()
    {
        return await _enrolledRepository.GetPendingEnrollmentsAsync();
    }




}