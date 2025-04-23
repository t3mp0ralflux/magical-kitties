using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Models.MagicalPowers;

public class MagicalPower : Endowment
{
    public List<MagicalPower> BonusFeatures { get; init; }
}