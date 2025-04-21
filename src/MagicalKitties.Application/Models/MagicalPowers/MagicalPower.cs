using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Models.MagicalPowers;

public class MagicalPower : Endowment
{
    public new List<MagicalPower> BonusFeatures { get; set; }
}