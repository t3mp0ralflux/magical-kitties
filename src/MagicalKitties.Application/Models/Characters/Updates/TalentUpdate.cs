namespace MagicalKitties.Application.Models.Characters.Updates;

public class TalentUpdate
{
    public required Guid CharacterId { get; set; }
    public required int TalentId { get; set; }
}