using Microsoft.EntityFrameworkCore;
using pattern_project.Domain.Entities;
using pattern_project.Domain.Enums;

namespace pattern_project.Database;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

  public DbSet<AppUser> Users => Set<AppUser>();

  public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();

  public DbSet<InpatientMedicalRecord> InpatientMedicalRecords => Set<InpatientMedicalRecord>();

  public DbSet<OutpatientMedicalRecord> OutpatientMedicalRecords => Set<OutpatientMedicalRecord>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<AppUser>(entity =>
    {
      entity.ToTable("Users");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
      entity.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
      entity.Property(x => x.Role).HasConversion<string>().HasMaxLength(20).IsRequired();
      entity.HasIndex(x => x.Username).IsUnique();

      entity.HasData(
        new { Id = 1, Username = "admin", PasswordHash = "Admin@123", Role = UserRole.Admin },
        new { Id = 2, Username = "patientA", PasswordHash = "User@123", Role = UserRole.User },
        new { Id = 3, Username = "patientB", PasswordHash = "User@123", Role = UserRole.User });
    });

    modelBuilder.Entity<MedicalRecord>(entity =>
    {
      entity.ToTable("MedicalRecords");
      entity.HasKey(x => x.Id);

      entity.Property(x => x.RecordCode).HasMaxLength(50).IsRequired();
      entity.Property(x => x.ExaminationDate).IsRequired();
      entity.Property(x => x.Diagnosis).HasMaxLength(2000).IsRequired();
      entity.Property(x => x.MedicalVerificationCode).HasMaxLength(100);
      entity.Property(x => x.CreatedAtUtc).IsRequired();
      entity.Property(x => x.DeletedBy).HasMaxLength(100);

      entity.HasIndex(x => x.RecordCode).IsUnique();
      entity.HasIndex(x => new { x.PatientId, x.ExaminationDate });

      entity.HasQueryFilter(x => !x.IsDeleted);

      entity.HasOne(x => x.Patient)
            .WithMany(x => x.MedicalRecords)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.ToTable(x => x.HasCheckConstraint(
        "CK_MedicalRecords_VerificationCode_WhenInfectious",
        "([IsDangerousInfectiousDisease] = 0 AND [MedicalVerificationCode] IS NULL) OR ([IsDangerousInfectiousDisease] = 1 AND [MedicalVerificationCode] IS NOT NULL)"));
    });

    modelBuilder.Entity<InpatientMedicalRecord>(entity =>
    {
      entity.ToTable("InpatientMedicalRecords");
      entity.Property(x => x.RoomNumber).HasMaxLength(20).IsRequired();
      entity.Property(x => x.BedNumber).HasMaxLength(20).IsRequired();
    });

    modelBuilder.Entity<OutpatientMedicalRecord>(entity =>
    {
      entity.ToTable("OutpatientMedicalRecords");
      entity.Property(x => x.EPrescriptionCode).HasMaxLength(100).IsRequired();
    });
  }
}
