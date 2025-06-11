using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Auth;
using MagicalKitties.Application.Repositories;

namespace MagicalKitties.Application.Services.Implementation;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IDateTimeProvider dateTimeProvider)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> ValidateRefreshToken(Guid accountId, AuthToken authToken, CancellationToken token = default)
    {
        RefreshToken? existingToken = await _refreshTokenRepository.GetRefreshToken(accountId, token);

        if (existingToken is null)
        {
            return false;
        }

        if (!string.Equals(existingToken.AccessToken, authToken.AccessToken, StringComparison.InvariantCultureIgnoreCase) || !string.Equals(existingToken.Token, authToken.RefreshToken, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        return existingToken.ExpirationUtc >= _dateTimeProvider.GetUtcNow();
    }

    public async Task<RefreshToken> UpsertRefreshToken(Account account, string accessToken, string refreshToken, CancellationToken token = default)
    { 
        RefreshToken existingToken = await _refreshTokenRepository.GetRefreshToken(account.Id, token) ?? new RefreshToken
                                                                                                         {
                                                                                                             Id = Guid.NewGuid(),
                                                                                                             AccountId = account.Id
                                                                                                         };

        existingToken.AccessToken = accessToken;
        existingToken.Token = refreshToken;
        existingToken.ExpirationUtc = _dateTimeProvider.GetUtcNow().AddDays(7);

        await _refreshTokenRepository.UpsertRefreshToken(existingToken, token);

        return existingToken;
    }

    public Task<bool> Exists(Guid accountId, CancellationToken token = default)
    {
        return _refreshTokenRepository.Exists(accountId, token);
    }

    public Task<RefreshToken?> GetRefreshToken(Guid accountId, CancellationToken token = default)
    {
        return _refreshTokenRepository.GetRefreshToken(accountId, token);
    }

    public Task<bool> DeleteRefreshToken(Guid accountId, CancellationToken token = default)
    {
        return _refreshTokenRepository.DeleteRefreshToken(accountId, token);
    }
}