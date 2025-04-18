namespace MagicalKitties.Contracts.Requests.Characters;

public class CharacterTalentUpdateRequest
{
    public required Guid CharacterId { get; set; }
    public required int TalentId { get; set; }
}