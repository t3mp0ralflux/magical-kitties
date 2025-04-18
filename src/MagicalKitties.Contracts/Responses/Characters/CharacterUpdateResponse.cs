namespace MagicalKitties.Contracts.Responses.Characters;

public class CharacterUpdateResponse
{
    public required Guid CharacterId { get; set; }
    public required string Message { get; set; }
}