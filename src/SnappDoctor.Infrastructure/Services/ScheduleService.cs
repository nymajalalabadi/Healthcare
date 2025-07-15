using Microsoft.EntityFrameworkCore;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Application.DTOs.Doctor;
using SnappDoctor.Core.Entities;
using SnappDoctor.Infrastructure.Data;

namespace SnappDoctor.Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly ApplicationDbContext _context;

    public ScheduleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DoctorScheduleDto>> GetDoctorScheduleAsync(int doctorId)
    {
        var schedules = await _context.DoctorSchedules
            .Where(s => s.DoctorId == doctorId)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync();

        return schedules.Select(s => new DoctorScheduleDto
        {
            Id = s.Id,
            DoctorId = s.DoctorId,
            DayOfWeek = s.DayOfWeek,
            IsActive = s.IsActive,
            StartTime = s.StartTime,
            EndTime = s.EndTime
        }).ToList();
    }

    public async Task<List<DoctorBreakTimeDto>> GetDoctorBreakTimesAsync(int doctorId)
    {
        var breakTimes = await _context.DoctorBreakTimes
            .Where(bt => bt.DoctorId == doctorId)
            .OrderBy(bt => bt.BreakType)
            .ToListAsync();

        return breakTimes.Select(bt => new DoctorBreakTimeDto
        {
            Id = bt.Id,
            DoctorId = bt.DoctorId,
            BreakType = bt.BreakType,
            Title = bt.Title,
            IsActive = bt.IsActive,
            StartTime = bt.StartTime,
            EndTime = bt.EndTime
        }).ToList();
    }

    public async Task<DoctorTimeSettingsDto?> GetDoctorTimeSettingsAsync(int doctorId)
    {
        var timeSettings = await _context.DoctorTimeSettings
            .FirstOrDefaultAsync(ts => ts.DoctorId == doctorId);

        if (timeSettings == null)
            return null;

        return new DoctorTimeSettingsDto
        {
            Id = timeSettings.Id,
            DoctorId = timeSettings.DoctorId,
            ConsultationDurationMinutes = timeSettings.ConsultationDurationMinutes,
            BreakBetweenConsultationsMinutes = timeSettings.BreakBetweenConsultationsMinutes,
            MaxDailyConsultations = timeSettings.MaxDailyConsultations
        };
    }

    public async Task<bool> UpdateDoctorScheduleAsync(int doctorId, UpdateScheduleDto updateDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Update doctor consultation preferences
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null) return false;

            doctor.OffersVoiceCall = updateDto.OffersVoiceCall;
            doctor.OffersVideoCall = updateDto.OffersVideoCall;
            doctor.OffersInPersonConsultation = updateDto.OffersInPersonConsultation;
            doctor.ConsultationFee = updateDto.ConsultationFee;
            doctor.UpdatedAt = DateTime.UtcNow;

            // Update schedules
            var existingSchedules = await _context.DoctorSchedules
                .Where(s => s.DoctorId == doctorId)
                .ToListAsync();

            _context.DoctorSchedules.RemoveRange(existingSchedules);

            foreach (var scheduleDto in updateDto.Schedules)
            {
                var schedule = new DoctorSchedule
                {
                    DoctorId = doctorId,
                    DayOfWeek = scheduleDto.DayOfWeek,
                    IsActive = scheduleDto.IsActive,
                    StartTime = scheduleDto.StartTime,
                    EndTime = scheduleDto.EndTime
                };
                _context.DoctorSchedules.Add(schedule);
            }

            // Update break times
            var existingBreakTimes = await _context.DoctorBreakTimes
                .Where(bt => bt.DoctorId == doctorId)
                .ToListAsync();

            _context.DoctorBreakTimes.RemoveRange(existingBreakTimes);

            foreach (var breakTimeDto in updateDto.BreakTimes)
            {
                var breakTime = new DoctorBreakTime
                {
                    DoctorId = doctorId,
                    BreakType = breakTimeDto.BreakType,
                    Title = breakTimeDto.Title,
                    IsActive = breakTimeDto.IsActive,
                    StartTime = breakTimeDto.StartTime,
                    EndTime = breakTimeDto.EndTime
                };
                _context.DoctorBreakTimes.Add(breakTime);
            }

            // Update time settings
            var existingTimeSettings = await _context.DoctorTimeSettings
                .FirstOrDefaultAsync(ts => ts.DoctorId == doctorId);

            if (existingTimeSettings != null)
            {
                existingTimeSettings.ConsultationDurationMinutes = updateDto.TimeSettings.ConsultationDurationMinutes;
                existingTimeSettings.BreakBetweenConsultationsMinutes = updateDto.TimeSettings.BreakBetweenConsultationsMinutes;
                existingTimeSettings.MaxDailyConsultations = updateDto.TimeSettings.MaxDailyConsultations;
                existingTimeSettings.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var timeSettings = new DoctorTimeSettings
                {
                    DoctorId = doctorId,
                    ConsultationDurationMinutes = updateDto.TimeSettings.ConsultationDurationMinutes,
                    BreakBetweenConsultationsMinutes = updateDto.TimeSettings.BreakBetweenConsultationsMinutes,
                    MaxDailyConsultations = updateDto.TimeSettings.MaxDailyConsultations
                };
                _context.DoctorTimeSettings.Add(timeSettings);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> InitializeDefaultScheduleAsync(int doctorId)
    {
        try
        {
            // Check if schedule already exists
            var existingSchedule = await _context.DoctorSchedules
                .AnyAsync(s => s.DoctorId == doctorId);

            if (existingSchedule) return true;

            // Create default schedule (Saturday to Thursday, 9 AM to 5 PM)
            var defaultSchedules = new List<DoctorSchedule>();
            
            for (int day = 6; day <= 4; day++) // Saturday (6) to Thursday (4)
            {
                var dayOfWeek = (DayOfWeek)(day % 7);
                defaultSchedules.Add(new DoctorSchedule
                {
                    DoctorId = doctorId,
                    DayOfWeek = dayOfWeek,
                    IsActive = dayOfWeek != DayOfWeek.Friday, // Friday is off by default
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(17, 0)
                });
            }

            _context.DoctorSchedules.AddRange(defaultSchedules);

            // Create default break times
            var defaultBreakTimes = new List<DoctorBreakTime>
            {
                new DoctorBreakTime
                {
                    DoctorId = doctorId,
                    BreakType = "Lunch",
                    Title = "ناهار",
                    IsActive = false,
                    StartTime = new TimeOnly(12, 30),
                    EndTime = new TimeOnly(13, 30)
                },
                new DoctorBreakTime
                {
                    DoctorId = doctorId,
                    BreakType = "Prayer",
                    Title = "نماز",
                    IsActive = false,
                    StartTime = new TimeOnly(18, 0),
                    EndTime = new TimeOnly(18, 15)
                }
            };

            _context.DoctorBreakTimes.AddRange(defaultBreakTimes);

            // Create default time settings
            var defaultTimeSettings = new DoctorTimeSettings
            {
                DoctorId = doctorId,
                ConsultationDurationMinutes = 30,
                BreakBetweenConsultationsMinutes = 5,
                MaxDailyConsultations = 20
            };

            _context.DoctorTimeSettings.Add(defaultTimeSettings);

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
} 