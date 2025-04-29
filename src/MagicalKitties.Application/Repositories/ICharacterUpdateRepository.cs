using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Repositories;

public interface ICharacterUpdateRepository
{
    Task<bool> UpdateNameAsync(DescriptionUpdate update, CancellationToken token = default);
    Task<bool> UpdateDescriptionAsync(DescriptionUpdate update, CancellationToken token = default);
    Task<bool> UpdateHometownAsync(DescriptionUpdate update, CancellationToken token = default);
    Task<bool> UpdateXPAsync(DescriptionUpdate update, CancellationToken token = default);
    Task<bool> UpdateCunningAsync(AttributeUpdate update, string cunningId, CancellationToken token = default);
    Task<bool> UpdateCuteAsync(AttributeUpdate update, string cuteId, CancellationToken token = default);
    Task<bool> UpdateFierceAsync(AttributeUpdate update, string fierceId, CancellationToken token = default);
    Task<bool> UpdateLevelAsync(AttributeUpdate update, CancellationToken token = default);
    Task<bool> UpdateFlawAsync(AttributeUpdate update, CancellationToken token = default);
    Task<bool> UpdateTalentAsync(AttributeUpdate update, CancellationToken token = default);
    Task<bool> UpdateMagicalPowerAsync(AttributeUpdate update, CancellationToken token = default);
    Task<bool> UpdateOwiesAsync(AttributeUpdate update, CancellationToken token = default);
    Task<bool> UpdateCurrentTreatsAsync(AttributeUpdate update, CancellationToken token = default);
}