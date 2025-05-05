using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Services;

public interface ICharacterUpdateService
{
    Task<bool> UpdateDescriptionAsync(DescriptionUpdate update, CancellationToken token = default);
    Task<bool> UpdateAttributeAsync(AttributeUpdate update, CancellationToken token = default);
}
