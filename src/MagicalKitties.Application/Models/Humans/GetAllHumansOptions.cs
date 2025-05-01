namespace MagicalKitties.Application.Models.Humans;

public class GetAllHumansOptions : SharedGetAllOptions
{
    public required Guid CharacterId { get; init; }
    public string? Name { get; init; }
}