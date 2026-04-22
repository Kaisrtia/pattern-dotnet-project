namespace pattern_project.Contracts.Responses;

public sealed class MedicalRecordResponse
{
  public int Id { get; set; }

  public string RecordType { get; set; } = string.Empty;

  public string RecordCode { get; set; } = string.Empty;

  public DateOnly ExaminationDate { get; set; }

  public string Diagnosis { get; set; } = string.Empty;

  public bool IsDangerousInfectiousDisease { get; set; }

  public string? MedicalVerificationCode { get; set; }

  public int PatientId { get; set; }

  public string? RoomNumber { get; set; }

  public string? BedNumber { get; set; }

  public string? EPrescriptionCode { get; set; }

  public DateTime CreatedAtUtc { get; set; }

  public bool IsDeleted { get; set; }

  public DateTime? DeletedAtUtc { get; set; }

  public string? DeletedBy { get; set; }
}
