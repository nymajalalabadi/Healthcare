using Microsoft.EntityFrameworkCore;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Core.Entities;
using SnappDoctor.Core.Enums;
using SnappDoctor.Infrastructure.Data;

namespace SnappDoctor.Infrastructure.Repositories;

public class ConsultationRepository : IConsultationRepository
{
    private readonly ApplicationDbContext _context;

    public ConsultationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Consultation?> GetByIdAsync(int id)
    {
        return await _context.Consultations
            .Include(c => c.User)
            .Include(c => c.Doctor)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Consultation>> GetAllAsync()
    {
        return await _context.Consultations
            .Include(c => c.User)
            .Include(c => c.Doctor)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Consultation>> GetUserConsultationsAsync(string userId)
    {
        return await _context.Consultations
            .Include(c => c.User)
            .Include(c => c.Doctor)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Consultation>> GetDoctorConsultationsAsync(int doctorId)
    {
        return await _context.Consultations
            .Include(c => c.User)
            .Include(c => c.Doctor)
            .Where(c => c.DoctorId == doctorId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Consultation>> GetConsultationsByStatusAsync(ConsultationStatus status)
    {
        return await _context.Consultations
            .Include(c => c.User)
            .Include(c => c.Doctor)
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Consultation> CreateAsync(Consultation consultation)
    {
        _context.Consultations.Add(consultation);
        await _context.SaveChangesAsync();
        return consultation;
    }

    public async Task<Consultation> UpdateAsync(Consultation consultation)
    {
        _context.Consultations.Update(consultation);
        await _context.SaveChangesAsync();
        return consultation;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var consultation = await _context.Consultations.FindAsync(id);
        if (consultation != null)
        {
            _context.Consultations.Remove(consultation);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
} 