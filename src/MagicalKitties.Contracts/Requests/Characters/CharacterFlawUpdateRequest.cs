namespace MagicalKitties.Contracts.Requests.Characters;

public class CharacterFlawUpdateRequest
{
    public required Guid CharacterId { get; set; }
    public required int FlawId { get; set; }
}