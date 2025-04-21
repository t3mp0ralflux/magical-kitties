using MagicalKitties.Contracts.Responses.Characters;

namespace MagicalKitties.Contracts.Responses.MagicalPowers;

public class MagicalPowerResponse : EndowmentResponse
{
    public required List<EndowmentResponse> BonusFeatures { get; init; }
}