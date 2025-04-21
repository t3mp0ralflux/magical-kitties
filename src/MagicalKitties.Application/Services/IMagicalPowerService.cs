using MagicalKitties.Application.Models.MagicalPowers;

namespace MagicalKitties.Application.Services;

public interface IMagicalPowerService
{
    Task<bool> CreateAsync(MagicalPower flaw, CancellationToken token = default);
    Task<MagicalPower?> GetByIdAsync(int id, CancellationToken token = default);
    Task<IEnumerable<MagicalPower>> GetAllAsync(GetAllMagicalPowersOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(GetAllMagicalPowersOptions options, CancellationToken token = default);
    Task<bool> UpdateAsync(MagicalPower flaw, CancellationToken token = default);
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}