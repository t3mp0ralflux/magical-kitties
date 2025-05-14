using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Models.Characters;

public class UpgradeRequest
{
    public required Guid AccountId { get; init; }
    public required UpgradeOption UpgradeOption { get; init; }
    public required Guid CharacterId { get; init; }
    public required Upgrade Upgrade { get; init; }
}