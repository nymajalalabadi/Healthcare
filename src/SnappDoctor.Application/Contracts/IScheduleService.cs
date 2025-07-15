using SnappDoctor.Application.DTOs.Doctor;
using SnappDoctor.Core.Entities;

namespace SnappDoctor.Application.Contracts;

public interface IScheduleService
{
    Task<List<DoctorScheduleDto>> GetDoctorScheduleAsync(int doctorId);
    Task<List<DoctorBreakTimeDto>> GetDoctorBreakTimesAsync(int doctorId);
    Task<DoctorTimeSettingsDto?> GetDoctorTimeSettingsAsync(int doctorId);
    Task<bool> UpdateDoctorScheduleAsync(int doctorId, UpdateScheduleDto updateDto);
    Task<bool> InitializeDefaultScheduleAsync(int doctorId);
} 