namespace MagicalKitties.Contracts.Requests.Characters;

public class CharacterUpdateRequest
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}