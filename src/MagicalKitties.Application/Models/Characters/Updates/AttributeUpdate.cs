namespace MagicalKitties.Application.Models.Characters.Updates;

public class AttributeUpdate
{
    public AttributeOption AttributeOption { get; init; }
    public required Guid AccountId { get; init; }
    public required Guid CharacterId { get; init; }
    public required int? Cute { get; init; }
    public required int? Cunning { get; init; }
    public required int? Fierce { get; init; }
    public required int? Level { get; init; }
    public required EndowmentChange? FlawChange { get; init; }
    public required EndowmentChange? TalentChange { get; init; }
    public required EndowmentChange? MagicalPowerChange { get; init; }
    public required int? CurrentOwies { get; init; }
    public required int? CurrentTreats { get; init; }
    public required int? CurrentInjuries { get; init; }
}

public class EndowmentChange
{
    public required int PreviousId { get; init; }
    public required int NewId { get; init; }
}