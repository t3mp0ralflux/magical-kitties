using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Models.Characters;

public class UpgradeRequest
{
    public required Guid AccountId { get; set; }
    public required UpgradeOption UpgradeOption { get; set; }
    public required Guid CharacterId { get; set; }
    public required Upgrade Upgrade { get; set; }
}