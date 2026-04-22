using pattern_project.Contracts.Requests;
using pattern_project.Contracts.Responses;
using pattern_project.Domain.Entities;
using pattern_project.Domain.Enums;
using pattern_project.Domain.Exceptions;
using pattern_project.Repositories.Interfaces;
using pattern_project.Services.Interfaces;

namespace pattern_project.Services;

public sealed class MedicalRecordService : IMedicalRecordService
{
  private readonly IMedicalRecordRepository _medicalRecordRepository;
  private readonly IUserRepository _userRepository;

  public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository, IUserRepository userRepository)
  {
    _medicalRecordRepository = medicalRecordRepository;
    _userRepository = userRepository;
  }

  public async Task<MedicalRecordResponse> CreateAsync(CreateMedicalRecordRequest request, CancellationToken cancellationToken)
  {
    ValidateCreateRequest(request);

    var patient = await _userRepository.FindByIdAsync(request.PatientId, cancellationToken)
        ?? throw new DomainValidationException("Patient does not exist.");

    if (patient.Role != UserRole.User)
    {
      throw new DomainValidationException("Medical record can only be assigned to a patient account.");
    }

    var existed = await _medicalRecordRepository.ExistsByRecordCodeAsync(request.RecordCode.Trim(), cancellationToken);
    if (existed)
    {
      throw new DomainValidationException("RecordCode already exists.");
    }

    var normalizedType = request.RecordType.Trim().ToLowerInvariant();
    MedicalRecord record = normalizedType switch
    {
      "inpatient" => new InpatientMedicalRecord(
          request.RecordCode.Trim(),
          request.ExaminationDate,
          request.Diagnosis.Trim(),
          request.IsDangerousInfectiousDisease,
          request.MedicalVerificationCode?.Trim(),
          request.PatientId,
          request.RoomNumber!.Trim(),
          request.BedNumber!.Trim()),
      "outpatient" => new OutpatientMedicalRecord(
          request.RecordCode.Trim(),
          request.ExaminationDate,
          request.Diagnosis.Trim(),
          request.IsDangerousInfectiousDisease,
          request.MedicalVerificationCode?.Trim(),
          request.PatientId,
          request.EPrescriptionCode!.Trim()),
      _ => throw new DomainValidationException("RecordType must be inpatient or outpatient.")
    };

    await _medicalRecordRepository.AddAsync(record, cancellationToken);
    await _medicalRecordRepository.SaveChangesAsync(cancellationToken);

    return MapToResponse(record);
  }

  public async Task<MedicalRecordResponse> GetByIdAsync(int recordId, int currentUserId, bool isAdmin, CancellationToken cancellationToken)
  {
    var record = await _medicalRecordRepository.FindByIdAsync(recordId, cancellationToken)
        ?? throw new NotFoundException("Medical record not found.");

    if (!isAdmin && record.PatientId != currentUserId)
    {
      throw new ForbiddenException("You are not allowed to access this medical record.");
    }

    return MapToResponse(record);
  }

  public async Task<IReadOnlyList<MedicalRecordResponse>> GetForCurrentUserAsync(int currentUserId, CancellationToken cancellationToken)
  {
    var records = await _medicalRecordRepository.FindByPatientIdAsync(currentUserId, cancellationToken);
    return records.Select(MapToResponse).ToList();
  }

  public async Task<IReadOnlyList<MedicalRecordResponse>> GetAllAsync(bool includeDeleted, CancellationToken cancellationToken)
  {
    var records = includeDeleted
        ? await _medicalRecordRepository.FindAllIncludingDeletedAsync(cancellationToken)
        : await _medicalRecordRepository.FindAllAsync(cancellationToken);

    return records.Select(MapToResponse).ToList();
  }

  public async Task ArchiveAsync(int recordId, string deletedBy, CancellationToken cancellationToken)
  {
    var record = await _medicalRecordRepository.FindByIdIncludingDeletedForUpdateAsync(recordId, cancellationToken)
        ?? throw new NotFoundException("Medical record not found.");

    if (record.IsDeleted)
    {
      throw new DomainValidationException("Medical record is already archived.");
    }

    var deletedByValue = string.IsNullOrWhiteSpace(deletedBy) ? "system" : deletedBy.Trim();
    record.MarkAsDeleted(deletedByValue);

    await _medicalRecordRepository.SaveChangesAsync(cancellationToken);
  }

  private static void ValidateCreateRequest(CreateMedicalRecordRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.RecordType))
    {
      throw new DomainValidationException("RecordType is required.");
    }

    if (string.IsNullOrWhiteSpace(request.RecordCode))
    {
      throw new DomainValidationException("RecordCode is required.");
    }

    if (request.ExaminationDate == DateOnly.MinValue)
    {
      throw new DomainValidationException("ExaminationDate is required.");
    }

    if (request.ExaminationDate > DateOnly.FromDateTime(DateTime.UtcNow))
    {
      throw new DomainValidationException("ExaminationDate cannot be in the future.");
    }

    if (string.IsNullOrWhiteSpace(request.Diagnosis))
    {
      throw new DomainValidationException("Diagnosis is required.");
    }

    if (request.PatientId <= 0)
    {
      throw new DomainValidationException("PatientId is invalid.");
    }

    if (request.IsDangerousInfectiousDisease && string.IsNullOrWhiteSpace(request.MedicalVerificationCode))
    {
      throw new DomainValidationException("MedicalVerificationCode is required for dangerous infectious disease.");
    }

    if (!request.IsDangerousInfectiousDisease && !string.IsNullOrWhiteSpace(request.MedicalVerificationCode))
    {
      throw new DomainValidationException("MedicalVerificationCode must be empty when disease is not marked infectious.");
    }

    var normalizedType = request.RecordType.Trim().ToLowerInvariant();
    if (normalizedType == "inpatient")
    {
      if (string.IsNullOrWhiteSpace(request.RoomNumber) || string.IsNullOrWhiteSpace(request.BedNumber))
      {
        throw new DomainValidationException("RoomNumber and BedNumber are required for inpatient record.");
      }

      if (!string.IsNullOrWhiteSpace(request.EPrescriptionCode))
      {
        throw new DomainValidationException("EPrescriptionCode is not allowed for inpatient record.");
      }
    }
    else if (normalizedType == "outpatient")
    {
      if (string.IsNullOrWhiteSpace(request.EPrescriptionCode))
      {
        throw new DomainValidationException("EPrescriptionCode is required for outpatient record.");
      }

      if (!string.IsNullOrWhiteSpace(request.RoomNumber) || !string.IsNullOrWhiteSpace(request.BedNumber))
      {
        throw new DomainValidationException("RoomNumber and BedNumber are not allowed for outpatient record.");
      }
    }
  }

  private static MedicalRecordResponse MapToResponse(MedicalRecord record)
  {
    var response = new MedicalRecordResponse
    {
      Id = record.Id,
      RecordCode = record.RecordCode,
      ExaminationDate = record.ExaminationDate,
      Diagnosis = record.Diagnosis,
      IsDangerousInfectiousDisease = record.IsDangerousInfectiousDisease,
      MedicalVerificationCode = record.MedicalVerificationCode,
      PatientId = record.PatientId,
      CreatedAtUtc = record.CreatedAtUtc,
      IsDeleted = record.IsDeleted,
      DeletedAtUtc = record.DeletedAtUtc,
      DeletedBy = record.DeletedBy
    };

    switch (record)
    {
      case InpatientMedicalRecord inpatient:
        response.RecordType = "Inpatient";
        response.RoomNumber = inpatient.RoomNumber;
        response.BedNumber = inpatient.BedNumber;
        break;
      case OutpatientMedicalRecord outpatient:
        response.RecordType = "Outpatient";
        response.EPrescriptionCode = outpatient.EPrescriptionCode;
        break;
      default:
        response.RecordType = "Unknown";
        break;
    }

    return response;
  }
}
