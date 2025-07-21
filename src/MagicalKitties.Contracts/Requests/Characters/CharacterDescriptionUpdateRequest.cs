namespace MagicalKitties.Contracts.Requests.Characters;

public class CharacterDescriptionUpdateRequest
{
    public required Guid CharacterId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Hometown { get; init; }
}