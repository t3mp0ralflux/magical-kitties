using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Flaws;

namespace MagicalKitties.Application.Services;

public interface IFlawService
{
    Task<bool> CreateAsync(Endowment flaw, CancellationToken token = default);
    Task<Endowment?> GetByIdAsync(int id, CancellationToken token = default);
    Task<IEnumerable<Endowment>> GetAllAsync(GetAllFlawsOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(GetAllFlawsOptions options, CancellationToken token = default);
    Task<bool> UpdateAsync(Endowment flaw, CancellationToken token = default);
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}
