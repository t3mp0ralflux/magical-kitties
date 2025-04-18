namespace MagicalKitties.Contracts.Requests.Characters;

public class CharacterLevelUpdateRequest
{
    public required Guid CharacterId { get; init; }
    public required int Level { get; init; }
}