using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.Rules;

namespace MagicalKitties.Application.Services;

public interface IRuleService
{
    Task<GameRules> GetAll(CancellationToken token = default);
}
