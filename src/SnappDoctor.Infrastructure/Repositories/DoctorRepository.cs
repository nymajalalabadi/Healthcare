using Microsoft.EntityFrameworkCore;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Core.Entities;
using SnappDoctor.Infrastructure.Data;

namespace SnappDoctor.Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Doctor?> GetByIdAsync(int id)
    {
        return await _context.Doctors
            .Include(d => d.Consultations)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Doctor>> GetAllAsync()
    {
        return await _context.Doctors
            .Where(d => d.IsActive)
            .OrderBy(d => d.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization)
    {
        return await _context.Doctors
            .Where(d => d.IsActive && d.Specialization.Contains(specialization))
            .OrderByDescending(d => d.Rating)
            .ToListAsync();
    }

    public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync()
    {
        return await _context.Doctors
            .Where(d => d.IsActive && d.IsAvailable)
            .OrderByDescending(d => d.Rating)
            .ToListAsync();
    }

    public async Task<Doctor> CreateAsync(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        return doctor;
    }

    public async Task<Doctor> UpdateAsync(Doctor doctor)
    {
        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync();
        return doctor;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor != null)
        {
            doctor.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
} 