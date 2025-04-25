using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Repositories;

public interface ICharacterUpdateRepository
{
    Task<bool> UpdateNameAsync(DescriptionUpdate update, CancellationToken token = default);
    Task<bool> UpdateDescriptionAsync(DescriptionUpdate update, CancellationToken token = default);
    Task<bool> UpdateHometownAsync(DescriptionUpdate update, CancellationToken token = default);
    Task<bool> UpdateXPAsync(DescriptionUpdate update, CancellationToken token = default);
}