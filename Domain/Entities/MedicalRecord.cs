namespace pattern_project.Domain.Entities;

public abstract class MedicalRecord
{
  protected MedicalRecord()
  {
  }

  protected MedicalRecord(
      string recordCode,
      DateOnly examinationDate,
      string diagnosis,
      bool isDangerousInfectiousDisease,
      string? medicalVerificationCode,
      int patientId)
  {
    RecordCode = recordCode;
    ExaminationDate = examinationDate;
    Diagnosis = diagnosis;
    IsDangerousInfectiousDisease = isDangerousInfectiousDisease;
    MedicalVerificationCode = medicalVerificationCode;
    PatientId = patientId;
    CreatedAtUtc = DateTime.UtcNow;
  }

  public int Id { get; private set; }

  public string RecordCode { get; private set; } = string.Empty;

  public DateOnly ExaminationDate { get; private set; }

  public string Diagnosis { get; private set; } = string.Empty;

  public bool IsDangerousInfectiousDisease { get; private set; }

  public string? MedicalVerificationCode { get; private set; }

  public int PatientId { get; private set; }

  public AppUser Patient { get; private set; } = null!;

  public DateTime CreatedAtUtc { get; private set; }

  public bool IsDeleted { get; private set; }

  public DateTime? DeletedAtUtc { get; private set; }

  public string? DeletedBy { get; private set; }

  public void MarkAsDeleted(string deletedBy)
  {
    if (IsDeleted)
    {
      return;
    }

    IsDeleted = true;
    DeletedAtUtc = DateTime.UtcNow;
    DeletedBy = deletedBy;
  }
}
