namespace MagicalKitties.Contracts.Requests.Humans;

public class HumanDescriptionUpdateRequest
{
    public required Guid CharacterId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
}