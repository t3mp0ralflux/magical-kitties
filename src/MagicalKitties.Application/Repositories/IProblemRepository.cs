using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.Humans.Updates;

namespace MagicalKitties.Application.Repositories;

public interface IProblemRepository
{
    Task<bool> CreateProblemAsync(Problem problem, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);
    Task<bool> UpdateSourceAsync(ProblemUpdate update, CancellationToken token = default);
    Task<bool> UpdateEmotionAsync(ProblemUpdate update, CancellationToken token = default);
    Task<bool> UpdateRankAsync(ProblemUpdate update, CancellationToken token = default);
    Task<bool> UpdateSolvedAsync(ProblemUpdate update, CancellationToken token = default);
    Task<bool> DeleteAsync(Guid problemId, CancellationToken token = default);
}