using MagicalKitties.Contracts.Responses.Characters;

namespace MagicalKitties.Contracts.Responses.MagicalPowers;

public class MagicalPowerResponse : EndowmentResponse
{
    public required List<MagicalPowerResponse> BonusFeatures { get; init; }
}