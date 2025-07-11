using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Services;

public interface ICharacterUpdateService
{
    Task<bool> UpdateDescriptionAsync(DescriptionOption option, DescriptionUpdate update, CancellationToken token = default);
    Task<bool> UpdateAttributeAsync(AttributeOption option, AttributeUpdate update, CancellationToken token = default);
    Task<bool> Reset(Guid characterId, CancellationToken token = default);
}