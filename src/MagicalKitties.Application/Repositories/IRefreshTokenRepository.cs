using MagicalKitties.Application.Models.Auth;

namespace MagicalKitties.Application.Repositories;

public interface IRefreshTokenRepository
{
    Task<bool> UpsertRefreshToken(RefreshToken refreshToken, CancellationToken token = default);
    Task<RefreshToken?> GetRefreshToken(Guid accountId, CancellationToken token = default);
    Task<bool> Exists(Guid accountId, CancellationToken token = default);
    Task<bool> DeleteRefreshToken(Guid accountId, CancellationToken token = default);
}
