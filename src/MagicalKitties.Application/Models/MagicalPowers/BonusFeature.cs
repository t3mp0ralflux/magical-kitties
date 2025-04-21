using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Models.MagicalPowers;

public class BonusFeature : Endowment
{
    public required bool Selected { get; set; } = false;
}