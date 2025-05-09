using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Services;

public interface ICharacterUpgradeService
{
    Task<bool> UpsertUpgradeAsync(UpgradeRequest update, CancellationToken token = default);
    Task<bool> RemoveUpgradeAsync(UpgradeRequest update, CancellationToken token = default);
}
