using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.Humans.Updates;

namespace MagicalKitties.Application.Repositories;

public interface IHumanRepository
{
    Task<bool> CreateAsync(Human human, CancellationToken token = default);
    Task<bool> CreateProblemAsync(Guid humanId, Problem problem, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);
    Task<Human?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken token = default);
    Task<IEnumerable<Human>> GetAllAsync(GetAllHumansOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(GetAllHumansOptions options, CancellationToken token = default);
    Task<bool> UpdateDescriptionAsync(DescriptionUpdate update, CancellationToken token = default);
    Task<bool> UpdateNameAsync(DescriptionUpdate update, CancellationToken token = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken token = default);
}
