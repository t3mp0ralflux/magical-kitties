using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Contracts.Responses.Flaws;
using MagicalKitties.Contracts.Responses.Humans;
using MagicalKitties.Contracts.Responses.MagicalPowers;
using MagicalKitties.Contracts.Responses.Talents;

namespace MagicalKitties.Contracts.Responses.Characters;

public class CharacterResponse
{
    public required Guid Id { get; init; }
    public required Guid AccountId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required FlawResponse? Flaw { get; init; }

    public required List<TalentResponse> Talents { get; init; }
    public required List<MagicalPowerResponse> MagicalPowers { get; init; }
    public required string Hometown { get; init; }
    public required List<HumanResponse> Humans { get; init; }

    public required int Level { get; init; } = 1;
    public required int CurrentXp { get; init; }
    public required int Cunning { get; init; }
    public required int Cute { get; init; }
    public required int Fierce { get; init; }
    public required int MaxOwies { get; init; } = 2;
    public required int CurrentOwies { get; init; }
    public required int StartingTreats { get; init; } = 2;
    public required int CurrentTreats { get; init; }
    public required int CurrentInjuries { get; init; }
    public required List<Upgrade> Upgrades { get; init; }
}