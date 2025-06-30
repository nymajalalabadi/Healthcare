using SnappDoctor.Core.Entities;
using SnappDoctor.Core.Enums;

namespace SnappDoctor.Application.Contracts;

public interface IConsultationRepository
{
    Task<Consultation?> GetByIdAsync(int id);
    Task<IEnumerable<Consultation>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Consultation>> GetByDoctorIdAsync(int doctorId);
    Task<IEnumerable<Consultation>> GetByStatusAsync(ConsultationStatus status);
    Task<Consultation> CreateAsync(Consultation consultation);
    Task<Consultation> UpdateAsync(Consultation consultation);
    Task<bool> DeleteAsync(int id);
} 