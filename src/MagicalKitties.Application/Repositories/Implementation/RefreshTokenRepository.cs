using System.Data;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models.Auth;

namespace MagicalKitties.Application.Repositories.Implementation;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RefreshTokenRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> UpsertRefreshToken(RefreshToken refreshToken, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into refreshtoken(id, account_id, access_token, token, expiration_utc)
                                                                                  values (@Id, @AccountId, @AccessToken, @Token, @ExpirationUtc)
                                                                                  on conflict (account_id) do update set access_token = @AccessToken, token = @Token, expiration_utc = @ExpirationUtc
                                                                                  """, new
                                                                                       {
                                                                                           refreshToken.Id,
                                                                                           refreshToken.AccountId,
                                                                                           refreshToken.AccessToken,
                                                                                           refreshToken.Token,
                                                                                           refreshToken.ExpirationUtc
                                                                                       }, cancellationToken: token));

        transaction.Commit();
        return result > 0;
    }

    public async Task<RefreshToken?> GetRefreshToken(Guid accountId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        RefreshToken? result = await connection.QuerySingleOrDefaultAsyncWithRetry<RefreshToken>(new CommandDefinition("""
                                                                                                                       select id, account_id, access_token, token, expiration_utc
                                                                                                                       from refreshtoken
                                                                                                                       where account_id = @accountId
                                                                                                                       limit 1
                                                                                                                       """, new
                                                                                                                            {
                                                                                                                                accountId
                                                                                                                            }, cancellationToken: token));

        return result;
    }

    public async Task<bool> Exists(Guid accountId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        int result = await connection.QuerySingleAsyncWithRetry<int>(new CommandDefinition("""
                                                                                           select count(id)
                                                                                           from refreshtoken
                                                                                           where account_id = @accountId
                                                                                           """, new { accountId }, cancellationToken: token));

        return result > 0;
    }

    public async Task<bool> DeleteRefreshToken(Guid accountId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  delete from refreshtoken
                                                                                  where account_id = @accountId
                                                                                  """, new { accountId }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }
}