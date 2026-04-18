namespace pattern_project.Dtos;

public sealed record CreateUpdateProductRequestDto(
  string Name,
  decimal Price,
  int Quantity,
  int CategoryId
);