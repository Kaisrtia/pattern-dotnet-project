using pattern_project.Models;
using pattern_project.Models.Requests;
using pattern_project.Models.Responses;
using pattern_project.Repositories;
using pattern_project.Services.Exceptions;

namespace pattern_project.Services;

public class TransactionService(
    ITransactionRepository transactionRepository,
    IAccountRepository accountRepository,
    IUserContextService userContextService) : ITransactionService
{
  public async Task<TransactionResponse> CreateInternalAsync(
      CreateInternalTransactionRequest request,
      CancellationToken cancellationToken)
  {
    if (request.Amount <= 0)
    {
      throw new DomainValidationException("Amount must be greater than zero.");
    }

    if (request.SourceAccountId == request.DestinationAccountId)
    {
      throw new DomainValidationException("Source and destination accounts must be different.");
    }

    var currentUserId = userContextService.GetRequiredUserId();
    var sourceAccount = await accountRepository.GetByIdAsync(request.SourceAccountId, cancellationToken)
                        ?? throw new NotFoundException("Source account was not found.");

    _ = await accountRepository.GetByIdAsync(request.DestinationAccountId, cancellationToken)
        ?? throw new NotFoundException("Destination account was not found.");

    if (!userContextService.IsAdmin() && sourceAccount.UserId != currentUserId)
    {
      throw new ForbiddenException("You are not allowed to transfer from this account.");
    }

    var transaction = new InternalTransaction(
        sourceAccount.UserId,
        request.Amount,
        request.SourceAccountId,
        request.DestinationAccountId);

    await transactionRepository.AddAsync(transaction, cancellationToken);
    await transactionRepository.SaveChangesAsync(cancellationToken);

    return ToResponse(transaction);
  }

  public async Task<TransactionResponse> CreateInterbankAsync(
      CreateInterbankTransactionRequest request,
      CancellationToken cancellationToken)
  {
    if (request.Amount <= 0)
    {
      throw new DomainValidationException("Amount must be greater than zero.");
    }

    var normalizedSwiftCode = request.SwiftCode.Trim().ToUpperInvariant();
    var currentUserId = userContextService.GetRequiredUserId();
    var sourceAccount = await accountRepository.GetByIdAsync(request.SourceAccountId, cancellationToken)
                        ?? throw new NotFoundException("Source account was not found.");

    if (!userContextService.IsAdmin() && sourceAccount.UserId != currentUserId)
    {
      throw new ForbiddenException("You are not allowed to transfer from this account.");
    }

    if (request.Amount > 50_000_000m && string.IsNullOrWhiteSpace(request.DigitalSignature))
    {
      throw new DomainValidationException(
          "Digital signature is required for interbank transactions above 50,000,000 VND.");
    }

    var transaction = new InterbankTransaction(
        sourceAccount.UserId,
        request.Amount,
        request.SourceAccountId,
        normalizedSwiftCode,
        request.DestinationBankName.Trim(),
        request.DestinationAccountNumber.Trim(),
        request.DigitalSignature?.Trim());

    await transactionRepository.AddAsync(transaction, cancellationToken);
    await transactionRepository.SaveChangesAsync(cancellationToken);

    return ToResponse(transaction);
  }

  public async Task<IReadOnlyCollection<TransactionResponse>> GetMyTransactionsAsync(CancellationToken cancellationToken)
  {
    var currentUserId = userContextService.GetRequiredUserId();
    var transactions = await transactionRepository.GetByOwnerAsync(currentUserId, cancellationToken);
    return transactions.Select(ToResponse).ToList();
  }

  public async Task<IReadOnlyCollection<TransactionResponse>> GetAllTransactionsAsync(CancellationToken cancellationToken)
  {
    if (!userContextService.IsAdmin())
    {
      throw new ForbiddenException("Only admin can view all transactions.");
    }

    var transactions = await transactionRepository.GetAllAsync(cancellationToken);
    return transactions.Select(ToResponse).ToList();
  }

  public async Task<TransactionResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
  {
    var transaction = await transactionRepository.GetByIdAsync(id, cancellationToken)
                      ?? throw new NotFoundException("Transaction not found.");

    var currentUserId = userContextService.GetRequiredUserId();
    if (!userContextService.IsAdmin() && transaction.OwnerUserId != currentUserId)
    {
      throw new ForbiddenException("You are not allowed to access this transaction.");
    }

    return ToResponse(transaction);
  }

  private static TransactionResponse ToResponse(Transaction transaction)
  {
    return transaction switch
    {
      InternalTransaction internalTransaction => new TransactionResponse
      {
        Id = internalTransaction.Id,
        Amount = internalTransaction.Amount,
        TransactionType = "Internal",
        OwnerUserId = internalTransaction.OwnerUserId,
        CreatedAt = internalTransaction.CreatedAt,
        SourceAccountId = internalTransaction.SourceAccountId,
        DestinationAccountId = internalTransaction.DestinationAccountId,
        HasDigitalSignature = false,
        RequiresDigitalSignature = false
      },
      InterbankTransaction interbankTransaction => new TransactionResponse
      {
        Id = interbankTransaction.Id,
        Amount = interbankTransaction.Amount,
        TransactionType = "Interbank",
        OwnerUserId = interbankTransaction.OwnerUserId,
        CreatedAt = interbankTransaction.CreatedAt,
        SourceAccountId = interbankTransaction.SourceAccountId,
        SwiftCode = interbankTransaction.SwiftCode,
        DestinationBankName = interbankTransaction.DestinationBankName,
        DestinationAccountNumber = interbankTransaction.DestinationAccountNumber,
        HasDigitalSignature = !string.IsNullOrWhiteSpace(interbankTransaction.DigitalSignature),
        RequiresDigitalSignature = interbankTransaction.RequiresDigitalSignature
      },
      _ => throw new DomainValidationException("Unknown transaction type.")
    };
  }
}
