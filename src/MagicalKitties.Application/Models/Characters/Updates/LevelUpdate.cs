namespace MagicalKitties.Application.Models.Characters.Updates;

public class LevelUpdate
{
    public required Guid CharacterId { get; init; }
    public required int Level { get; init; }
}