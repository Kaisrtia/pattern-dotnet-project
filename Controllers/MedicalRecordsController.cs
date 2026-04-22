using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pattern_project.Contracts.Requests;
using pattern_project.Extensions;
using pattern_project.Services.Interfaces;

namespace pattern_project.Controllers;

[ApiController]
[Route("api/medical-records")]
[Authorize]
public sealed class MedicalRecordsController : ControllerBase
{
  private readonly IMedicalRecordService _medicalRecordService;

  public MedicalRecordsController(IMedicalRecordService medicalRecordService)
  {
    _medicalRecordService = medicalRecordService;
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Create([FromBody] CreateMedicalRecordRequest request, CancellationToken cancellationToken)
  {
    var response = await _medicalRecordService.CreateAsync(request, cancellationToken);
    return CreatedAtAction(nameof(GetById), new { recordId = response.Id }, response);
  }

  [HttpGet("{recordId:int}")]
  public async Task<IActionResult> GetById([FromRoute] int recordId, CancellationToken cancellationToken)
  {
    var currentUserId = User.GetRequiredUserId();
    var isAdmin = User.IsInRole("Admin");

    var response = await _medicalRecordService.GetByIdAsync(recordId, currentUserId, isAdmin, cancellationToken);
    return Ok(response);
  }

  [HttpGet("my-history")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetMyHistory(CancellationToken cancellationToken)
  {
    var currentUserId = User.GetRequiredUserId();
    var response = await _medicalRecordService.GetForCurrentUserAsync(currentUserId, cancellationToken);
    return Ok(response);
  }

  [HttpDelete("{recordId:int}")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> ArchiveById([FromRoute] int recordId, CancellationToken cancellationToken)
  {
    var deletedBy = User.Identity?.Name ?? "admin";
    await _medicalRecordService.ArchiveAsync(recordId, deletedBy, cancellationToken);
    return NoContent();
  }

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted, CancellationToken cancellationToken)
  {
    var response = await _medicalRecordService.GetAllAsync(includeDeleted, cancellationToken);
    return Ok(response);
  }
}
