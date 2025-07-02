using SnappDoctor.Core.Entities;
using SnappDoctor.Core.Enums;

namespace SnappDoctor.Application.Contracts;

public interface IConsultationRepository
{
    Task<Consultation?> GetByIdAsync(int id);
    Task<IEnumerable<Consultation>> GetAllAsync();
    Task<IEnumerable<Consultation>> GetUserConsultationsAsync(string userId);
    Task<IEnumerable<Consultation>> GetDoctorConsultationsAsync(int doctorId);
    Task<IEnumerable<Consultation>> GetConsultationsByStatusAsync(ConsultationStatus status);
    Task<Consultation> CreateAsync(Consultation consultation);
    Task<Consultation> UpdateAsync(Consultation consultation);
    Task<bool> DeleteAsync(int id);
} 