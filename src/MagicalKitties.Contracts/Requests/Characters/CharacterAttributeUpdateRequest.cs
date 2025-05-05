namespace MagicalKitties.Contracts.Requests.Characters;

public class CharacterAttributeUpdateRequest
{
    public required Guid CharacterId { get; init; }
    public int? Cute { get; init; }
    public int? Cunning { get; init; }
    public int? Fierce { get; init; }
    public int? Level { get; init; }
    public EndowmentChangeRequest? FlawChange { get; init; }
    public EndowmentChangeRequest? TalentChange { get; init; }
    public EndowmentChangeRequest? MagicalPowerChange { get; init; }
    
    public int? CurrentOwies { get; init; }
    public int? CurrentTreats { get; init; }
    public int? CurrentInjuries { get; init; }
}

public class EndowmentChangeRequest
{
    public required int PreviousId { get; init; }
    public required int NewId { get; init; }
}