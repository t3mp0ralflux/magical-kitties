namespace MagicalKitties.Contracts.Requests.Characters;

public class CharacterMagicalPowerUpdateRequest
{
    public required Guid CharacterId { get; set; }
    public required int MagicalPowerId { get; set; }
}