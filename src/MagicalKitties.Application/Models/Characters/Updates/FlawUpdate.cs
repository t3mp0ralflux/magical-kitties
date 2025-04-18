namespace MagicalKitties.Application.Models.Characters.Updates;

public class FlawUpdate
{
    public required Guid CharacterId { get; set; }
    public required int FlawId { get; set; }
}