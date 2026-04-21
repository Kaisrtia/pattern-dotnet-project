using pattern_project.Models.Requests;
using pattern_project.Models.Responses;

namespace pattern_project.Services;

public interface ITransactionService
{
  Task<TransactionResponse> CreateInternalAsync(CreateInternalTransactionRequest request, CancellationToken cancellationToken);
  Task<TransactionResponse> CreateInterbankAsync(CreateInterbankTransactionRequest request, CancellationToken cancellationToken);
  Task<IReadOnlyCollection<TransactionResponse>> GetMyTransactionsAsync(CancellationToken cancellationToken);
  Task<IReadOnlyCollection<TransactionResponse>> GetAllTransactionsAsync(CancellationToken cancellationToken);
  Task<TransactionResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
