namespace pattern_project.Dtos;

public sealed record ProductResponseDto(
  int Id,
  string Name,
  decimal Price,
  int Quantity,
  int CategoryId,
  string CategoryName
);