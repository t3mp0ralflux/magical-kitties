namespace MagicalKitties.Contracts.Responses.Characters;

public class CharacterResponse
{
    public required Guid Id { get; init; }
    public required Guid AccountId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    
    public required List<AttributeResponse> Attributes {get;init;}
    public required EndowmentResponse? Flaw { get; init; }
    public required EndowmentResponse? Talent { get; init; }
    public required List<EndowmentResponse> MagicalPowers { get; init; }
    public required string Hometown { get; init; }
    public required HumanResponse? Human { get; init; }
    
    public required int Level { get; init; } = 1;
    public required int CurrentXp { get; init; }
    public required int MaxOwies { get; init; } = 2;
    public required int CurrentOwies { get; init; }
    public required int StartingTreats { get; init; } = 2;
    public required int CurrentTreats { get; init; }
    public required int CurrentInjuries { get; init; }
}