using Microsoft.EntityFrameworkCore;
using pattern_project.Database;
using pattern_project.Domain.Entities;
using pattern_project.Repositories.Interfaces;

namespace pattern_project.Repositories;

public sealed class MedicalRecordRepository : IMedicalRecordRepository
{
  private readonly AppDbContext _dbContext;

  public MedicalRecordRepository(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task AddAsync(MedicalRecord record, CancellationToken cancellationToken)
  {
    await _dbContext.MedicalRecords.AddAsync(record, cancellationToken);
  }

  public Task<MedicalRecord?> FindByIdAsync(int recordId, CancellationToken cancellationToken)
  {
    return _dbContext.MedicalRecords
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == recordId, cancellationToken);
  }

  public async Task<IReadOnlyList<MedicalRecord>> FindByPatientIdAsync(int patientId, CancellationToken cancellationToken)
  {
    return await _dbContext.MedicalRecords
        .AsNoTracking()
        .Where(x => x.PatientId == patientId)
        .OrderByDescending(x => x.ExaminationDate)
        .ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyList<MedicalRecord>> FindAllAsync(CancellationToken cancellationToken)
  {
    return await _dbContext.MedicalRecords
        .AsNoTracking()
        .OrderByDescending(x => x.ExaminationDate)
        .ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyList<MedicalRecord>> FindAllIncludingDeletedAsync(CancellationToken cancellationToken)
  {
    return await _dbContext.MedicalRecords
        .IgnoreQueryFilters()
        .AsNoTracking()
        .OrderByDescending(x => x.ExaminationDate)
        .ToListAsync(cancellationToken);
  }

  public Task<MedicalRecord?> FindByIdIncludingDeletedForUpdateAsync(int recordId, CancellationToken cancellationToken)
  {
    return _dbContext.MedicalRecords
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(x => x.Id == recordId, cancellationToken);
  }

  public Task<bool> ExistsByRecordCodeAsync(string recordCode, CancellationToken cancellationToken)
  {
    return _dbContext.MedicalRecords.AnyAsync(x => x.RecordCode == recordCode, cancellationToken);
  }

  public Task SaveChangesAsync(CancellationToken cancellationToken)
  {
    return _dbContext.SaveChangesAsync(cancellationToken);
  }
}
