using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Auth;

namespace MagicalKitties.Application.Services;

public interface IRefreshTokenService
{
    Task<bool> ValidateRefreshToken(Guid accountId, AuthToken authToken, CancellationToken token = default);
    Task<RefreshToken> UpsertRefreshToken(Account account, string accessToken, string refreshToken, CancellationToken token = default);
    Task<bool> Exists(Guid accountId, CancellationToken token = default);
    Task<RefreshToken?> GetRefreshToken(Guid accountId, CancellationToken token = default);
    Task<bool> DeleteRefreshToken(Guid accountId, CancellationToken token = default);
}