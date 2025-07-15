using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SnappDoctor.Core.Entities;

namespace SnappDoctor.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Consultation> Consultations { get; set; }
    public DbSet<OtpCode> OtpCodes { get; set; }
    public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
    public DbSet<DoctorBreakTime> DoctorBreakTimes { get; set; }
    public DbSet<DoctorTimeSettings> DoctorTimeSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // User entity configuration
        builder.Entity<User>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProfilePictureUrl).HasMaxLength(500);
            entity.HasIndex(e => e.PhoneNumber).IsUnique();
        });

        // Doctor entity configuration
        builder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Specialization).HasMaxLength(100).IsRequired();
            entity.Property(e => e.MedicalLicenseNumber).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.ProfilePictureUrl).HasMaxLength(500);
            entity.Property(e => e.Bio).HasMaxLength(1000);
            entity.Property(e => e.Rating).HasPrecision(3, 2);
            entity.HasIndex(e => e.MedicalLicenseNumber).IsUnique().HasFilter("[MedicalLicenseNumber] IS NOT NULL");
            // Removed unique constraint on PhoneNumber to avoid conflict with AspNetUsers
            entity.HasIndex(e => e.Email).IsUnique().HasFilter("[Email] IS NOT NULL");

            // Relationship with User
            entity.HasOne(e => e.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Consultation entity configuration
        builder.Entity<Consultation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PatientSymptoms).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.DoctorNotes).HasMaxLength(2000);
            entity.Property(e => e.Prescription).HasMaxLength(2000);
            entity.Property(e => e.Fee).HasPrecision(10, 2);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Consultations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Doctor)
                .WithMany(d => d.Consultations)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // OtpCode entity configuration
        builder.Entity<OtpCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Purpose).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.OtpCodes)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DoctorSchedule entity configuration
        builder.Entity<DoctorSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DayOfWeek).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(e => e.Doctor)
                .WithMany(d => d.Schedules)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.DoctorId, e.DayOfWeek }).IsUnique();
        });

        // DoctorBreakTime entity configuration
        builder.Entity<DoctorBreakTime>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BreakType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(100).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(e => e.Doctor)
                .WithMany(d => d.BreakTimes)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DoctorTimeSettings entity configuration
        builder.Entity<DoctorTimeSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ConsultationDurationMinutes).HasDefaultValue(30);
            entity.Property(e => e.BreakBetweenConsultationsMinutes).HasDefaultValue(5);
            entity.Property(e => e.MaxDailyConsultations).HasDefaultValue(20);

            entity.HasOne(e => e.Doctor)
                .WithOne(d => d.TimeSettings)
                .HasForeignKey<DoctorTimeSettings>(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedData(builder);
    }

    private void SeedData(ModelBuilder builder)
    {
        // Sample doctors data
        builder.Entity<Doctor>().HasData(
            new Doctor
            {
                Id = 1,
                FirstName = "دکتر سارا",
                LastName = "صادقی",
                Specialization = "زنان و زایمان",
                MedicalLicenseNumber = "12345",
                PhoneNumber = "09123456789",
                Email = "sara.sadeghi@example.com",
                Bio = "متخصص زنان و زایمان با بیش از 10 سال تجربه",
                YearsOfExperience = 10,
                Rating = 4.6m,
                ReviewCount = 250,
                IsAvailable = true,
                IsActive = true
            },
            new Doctor
            {
                Id = 2,
                FirstName = "دکتر محمد",
                LastName = "فرضی‌پور",
                Specialization = "پزشک عمومی",
                MedicalLicenseNumber = "54321",
                PhoneNumber = "09123456790",
                Email = "mohammad.farzipour@example.com",
                Bio = "پزشک عمومی با تجربه در مشاوره‌های آنلاین",
                YearsOfExperience = 8,
                Rating = 4.9m,
                ReviewCount = 300,
                IsAvailable = true,
                IsActive = true
            },
            new Doctor
            {
                Id = 3,
                FirstName = "دکتر میلاد",
                LastName = "مظفری",
                Specialization = "داخلی",
                MedicalLicenseNumber = "67890",
                PhoneNumber = "09123456791",
                Email = "milad.mozaffari@example.com",
                Bio = "متخصص بیماری‌های داخلی",
                YearsOfExperience = 12,
                Rating = 4.7m,
                ReviewCount = 180,
                IsAvailable = true,
                IsActive = true
            }
        );
    }
} 