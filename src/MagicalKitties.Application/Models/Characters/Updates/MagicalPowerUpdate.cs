namespace MagicalKitties.Application.Models.Characters.Updates;

public class MagicalPowerUpdate
{
    public required Guid CharacterId { get; init; }
    public required int MagicalPowerId { get; init; }
}