namespace pattern_project.Domain.Entities;

public sealed class OutpatientMedicalRecord : MedicalRecord
{
  private OutpatientMedicalRecord()
  {
  }

  public OutpatientMedicalRecord(
      string recordCode,
      DateOnly examinationDate,
      string diagnosis,
      bool isDangerousInfectiousDisease,
      string? medicalVerificationCode,
      int patientId,
      string ePrescriptionCode)
      : base(recordCode, examinationDate, diagnosis, isDangerousInfectiousDisease, medicalVerificationCode, patientId)
  {
    EPrescriptionCode = ePrescriptionCode;
  }

  public string EPrescriptionCode { get; private set; } = string.Empty;
}
