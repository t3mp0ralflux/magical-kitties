using MagicalKitties.Contracts.Responses.Characters;

namespace MagicalKitties.Contracts.Responses.MagicalPowers;

public class MagicalPowerResponse : EndowmentResponse
{
    public required bool IsPrimary { get; init; }
    public List<MagicalPowerResponse> BonusFeatures { get; init; } = [];
}