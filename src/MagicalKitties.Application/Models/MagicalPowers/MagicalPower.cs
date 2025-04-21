using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Models.MagicalPowers;

public class MagicalPower : Endowment
{
    public required List<Endowment> BonusFeatures { get; set; }
}