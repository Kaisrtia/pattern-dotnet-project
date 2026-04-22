using pattern_project.Domain.Entities;

namespace pattern_project.Repositories.Interfaces;

public interface IMedicalRecordRepository
{
  Task AddAsync(MedicalRecord record, CancellationToken cancellationToken);

  Task<MedicalRecord?> FindByIdAsync(int recordId, CancellationToken cancellationToken);

  Task<IReadOnlyList<MedicalRecord>> FindByPatientIdAsync(int patientId, CancellationToken cancellationToken);

  Task<IReadOnlyList<MedicalRecord>> FindAllAsync(CancellationToken cancellationToken);

  Task<IReadOnlyList<MedicalRecord>> FindAllIncludingDeletedAsync(CancellationToken cancellationToken);

  Task<MedicalRecord?> FindByIdIncludingDeletedForUpdateAsync(int recordId, CancellationToken cancellationToken);

  Task<bool> ExistsByRecordCodeAsync(string recordCode, CancellationToken cancellationToken);

  Task SaveChangesAsync(CancellationToken cancellationToken);
}
