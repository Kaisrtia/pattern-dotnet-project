using pattern_project.Contracts.Requests;
using pattern_project.Contracts.Responses;

namespace pattern_project.Services.Interfaces;

public interface IMedicalRecordService
{
  Task<MedicalRecordResponse> CreateAsync(CreateMedicalRecordRequest request, CancellationToken cancellationToken);

  Task<MedicalRecordResponse> GetByIdAsync(int recordId, int currentUserId, bool isAdmin, CancellationToken cancellationToken);

  Task<IReadOnlyList<MedicalRecordResponse>> GetForCurrentUserAsync(int currentUserId, CancellationToken cancellationToken);

  Task<IReadOnlyList<MedicalRecordResponse>> GetAllAsync(bool includeDeleted, CancellationToken cancellationToken);

  Task ArchiveAsync(int recordId, string deletedBy, CancellationToken cancellationToken);
}
