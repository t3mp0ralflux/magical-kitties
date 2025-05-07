namespace MagicalKitties.Application.Models.Characters.Updates;

public class AttributeUpdate
{
    public required Guid AccountId { get; init; }
    public required Guid CharacterId { get; init; }
    public int? Cute { get; init; }
    public int? Cunning { get; init; }
    public int? Fierce { get; init; }
    public int? Level { get; init; }
    public EndowmentChange? FlawChange { get; init; }
    public EndowmentChange? TalentChange { get; init; }
    public EndowmentChange? MagicalPowerChange { get; init; }
    public int? CurrentOwies { get; init; }
    public int? CurrentTreats { get; init; }
    public int? CurrentInjuries { get; init; }
    public bool? Incapacitated { get; init; }
}

public class EndowmentChange
{
    public required int PreviousId { get; init; }
    public required int NewId { get; init; }
}