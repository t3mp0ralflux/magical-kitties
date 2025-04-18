using MagicalKitties.Application.Models.Flaws;

namespace MagicalKitties.Application.Services;

public interface IFlawService
{
    Task<bool> CreateAsync(Flaw flaw, CancellationToken token = default);
    Task<Flaw?> GetByIdAsync(int id, CancellationToken token = default);
    Task<IEnumerable<Flaw>> GetAllAsync(GetAllFlawsOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(GetAllFlawsOptions options, CancellationToken token = default);
    Task<bool> UpdateAsync(Flaw flaw, CancellationToken token = default);
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}