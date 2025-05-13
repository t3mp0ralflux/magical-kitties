using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.Humans.Updates;

namespace MagicalKitties.Application.Services;

public interface IHumanService
{
    Task<Human> CreateAsync(Guid characterId, CancellationToken token = default);
    Task<bool> CreateProblemAsync(Guid humanId, CancellationToken token = default);
    Task<Human?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<Human>> GetAllAsync(GetAllHumansOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(GetAllHumansOptions options, CancellationToken token = default);
    Task<bool> UpdateDescriptionAsync(DescriptionUpdate update, CancellationToken token = default);
    Task<bool> UpdateProblemAsync(ProblemUpdate update, CancellationToken token = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken token = default);
    Task<bool> DeleteProblemAsync(Guid humanId, Guid problemId, CancellationToken token = default);
}