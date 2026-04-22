namespace pattern_project.Domain.Entities;

public sealed class InpatientMedicalRecord : MedicalRecord
{
  private InpatientMedicalRecord()
  {
  }

  public InpatientMedicalRecord(
      string recordCode,
      DateOnly examinationDate,
      string diagnosis,
      bool isDangerousInfectiousDisease,
      string? medicalVerificationCode,
      int patientId,
      string roomNumber,
      string bedNumber)
      : base(recordCode, examinationDate, diagnosis, isDangerousInfectiousDisease, medicalVerificationCode, patientId)
  {
    RoomNumber = roomNumber;
    BedNumber = bedNumber;
  }

  public string RoomNumber { get; private set; } = string.Empty;

  public string BedNumber { get; private set; } = string.Empty;
}
