using Microsoft.EntityFrameworkCore;
using pattern_project.Database;
using pattern_project.Models;

namespace pattern_project.Repositories;

public class TransactionRepository(AppDbContext dbContext) : ITransactionRepository
{
  public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken)
  {
    await dbContext.Transactions.AddAsync(transaction, cancellationToken);
  }

  public Task<Transaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken)
  {
    return dbContext.Transactions
        .Include(t => t.OwnerUser)
        .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);
  }

  public Task<List<Transaction>> GetByOwnerAsync(long ownerUserId, CancellationToken cancellationToken)
  {
    return dbContext.Transactions
        .Where(t => t.OwnerUserId == ownerUserId)
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync(cancellationToken);
  }

  public Task<List<Transaction>> GetAllAsync(CancellationToken cancellationToken)
  {
    return dbContext.Transactions
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync(cancellationToken);
  }

  public Task SaveChangesAsync(CancellationToken cancellationToken)
  {
    return dbContext.SaveChangesAsync(cancellationToken);
  }
}
