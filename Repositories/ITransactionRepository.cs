using pattern_project.Models;

namespace pattern_project.Repositories;

public interface ITransactionRepository
{
  Task AddAsync(Transaction transaction, CancellationToken cancellationToken);
  Task<Transaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken);
  Task<List<Transaction>> GetByOwnerAsync(long ownerUserId, CancellationToken cancellationToken);
  Task<List<Transaction>> GetAllAsync(CancellationToken cancellationToken);
  Task SaveChangesAsync(CancellationToken cancellationToken);
}
