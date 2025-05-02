using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QtechBackend.Context;
using QtechBackendv2.Models;
using QtechBackendv2.RepositoryInterface;
using System;

public class EnrolledRepository : IEnrolledRepository
{
    private readonly ElearningContext _context;

    public EnrolledRepository(ElearningContext context)
    {
        _context = context;
    }
    public async Task<List<UserPlaylistDto>> GetUserPlaylistsAsync()
    {
        return await _context.Enrolleds
            .Select(e => new UserPlaylistDto
            {
                UserId = e.UserEmail,
                PlaylistId = e.PlaylistId,
                EnrollStatus=e.EnrollStatus
            })
            .ToListAsync();
    }
    public async Task<List<int>> GetPlaylistIdsByUserEmailAsync(string userEmail)
    {
        return await _context.Enrolleds
            .Where(e => e.UserEmail == userEmail && e.EnrollStatus)
            .Select(e => e.PlaylistId)
            .ToListAsync();
    }

    public async Task<Enrolled> CreateEnrolledAsync(Enrolled enrolled)
    {
        
        var existingEnrollment = await _context.Enrolleds
            .FirstOrDefaultAsync(e => e.UserEmail == enrolled.UserEmail && e.PlaylistId == enrolled.PlaylistId);

        if (existingEnrollment != null)
        {
            return existingEnrollment;
        }

        _context.Enrolleds.Add(enrolled);
        await _context.SaveChangesAsync();
        return enrolled;
    }


    public async Task DeleteEnrolledAsync(string userEmail, int playlistId)
    {
        var enrolled = await _context.Enrolleds
            .FirstOrDefaultAsync(e => e.UserEmail == userEmail && e.PlaylistId == playlistId);
        if (enrolled != null)
        {
            _context.Enrolleds.Remove(enrolled);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Enrolled> UpdateEnrolledAsync(Enrolled enrolled)
    {
        _context.Enrolleds.Update(enrolled);
        await _context.SaveChangesAsync();
        return enrolled;
    }

    public async Task ApproveAllEnrollmentsAsync()
    {
        var enrollmentsToUpdate = await _context.Enrolleds
            .Where(e => !e.EnrollStatus)
            .ToListAsync();

        foreach (var enrollment in enrollmentsToUpdate)
        {
            enrollment.EnrollStatus = true;
        }

        await _context.SaveChangesAsync();
    }


    public async Task<List<UserPlaylistDto>> GetPendingEnrollmentsAsync()
    {
        return await _context.Enrolleds
            .Where(e => !e.EnrollStatus) 
            .Select(e => new UserPlaylistDto
            {
                UserId = e.UserEmail,
                PlaylistId = e.PlaylistId, 
                EnrollStatus = e.EnrollStatus
            })
            .ToListAsync();
    }
    public async Task<IEnumerable<UserPlaylistDto>> GetApprovedEnrollmentsAsync()

    {

        var approvedEnrollments = await _context.Enrolleds

            .Where(e => e.EnrollStatus == true)

            .Select(e => new UserPlaylistDto

            {

                UserId = e.UserEmail,

                PlaylistId = e.PlaylistId,

                EnrollStatus = e.EnrollStatus

            })

            .ToListAsync();

        return approvedEnrollments;

    }


}