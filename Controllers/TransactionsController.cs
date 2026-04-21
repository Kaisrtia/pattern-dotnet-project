using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pattern_project.Models.Requests;
using pattern_project.Models.Responses;
using pattern_project.Services;

namespace pattern_project.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "UserOrAdmin")]
public class TransactionsController(ITransactionService transactionService) : ControllerBase
{
  [HttpGet("me")]
  public async Task<ActionResult<IReadOnlyCollection<TransactionResponse>>> GetMyTransactions(
      CancellationToken cancellationToken)
  {
    var result = await transactionService.GetMyTransactionsAsync(cancellationToken);
    return Ok(result);
  }

  [HttpGet]
  [Authorize(Policy = "AdminOnly")]
  public async Task<ActionResult<IReadOnlyCollection<TransactionResponse>>> GetAllTransactions(
      CancellationToken cancellationToken)
  {
    var result = await transactionService.GetAllTransactionsAsync(cancellationToken);
    return Ok(result);
  }

  [HttpGet("{id:guid}")]
  public async Task<ActionResult<TransactionResponse>> GetById(Guid id, CancellationToken cancellationToken)
  {
    var result = await transactionService.GetByIdAsync(id, cancellationToken);
    return Ok(result);
  }

  [HttpPost("internal")]
  public async Task<ActionResult<TransactionResponse>> CreateInternal(
      [FromBody] CreateInternalTransactionRequest request,
      CancellationToken cancellationToken)
  {
    var result = await transactionService.CreateInternalAsync(request, cancellationToken);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
  }

  [HttpPost("interbank")]
  public async Task<ActionResult<TransactionResponse>> CreateInterbank(
      [FromBody] CreateInterbankTransactionRequest request,
      CancellationToken cancellationToken)
  {
    var result = await transactionService.CreateInterbankAsync(request, cancellationToken);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
  }
}
