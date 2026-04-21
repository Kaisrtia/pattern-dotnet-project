using Microsoft.AspNetCore.Mvc;
using pattern_project.Models;
using pattern_project.Services;

namespace pattern_project.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AssetsController : ControllerBase
{
  private readonly IAssetService _assetService;

  public AssetsController(IAssetService assetService)
  {
    _assetService = assetService;
  }

  [HttpGet]
  public async Task<ActionResult<IReadOnlyList<AssetView>>> GetAll(CancellationToken cancellationToken)
  {
    var result = await _assetService.GetAllAsync(cancellationToken);
    return Ok(result);
  }

  [HttpGet("{id:guid}")]
  public async Task<ActionResult<AssetView>> GetById(Guid id, CancellationToken cancellationToken)
  {
    var result = await _assetService.GetByIdAsync(id, cancellationToken);
    return result is null ? NotFound() : Ok(result);
  }

  [HttpPost("cash")]
  public async Task<ActionResult<AssetView>> CreateCash(
      [FromBody] CreateCashAssetRequest request,
      CancellationToken cancellationToken)
  {
    var created = await _assetService.CreateCashAsync(request, cancellationToken);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
  }

  [HttpPost("crypto")]
  public async Task<ActionResult<AssetView>> CreateCrypto(
      [FromBody] CreateCryptoAssetRequest request,
      CancellationToken cancellationToken)
  {
    var created = await _assetService.CreateCryptoAsync(request, cancellationToken);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
  }
}
