using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Repositories;

public interface IUpgradeRepository
{
    Task<List<UpgradeRule>> GetRulesAsync(CancellationToken token = default);
    Task<bool> UpsertUpgradesAsync(Guid characterId, List<Upgrade> upgrades, CancellationToken token = default);
}
