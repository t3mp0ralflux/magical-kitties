using System.Data;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Repositories.Implementation;

public class CharacterUpdateRepository : ICharacterUpdateRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public CharacterUpdateRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> UpdateNameAsync(DescriptionUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character
                                                                                  set name = @Name
                                                                                  where id = @CharacterId
                                                                                  and account_id = @AccountId
                                                                                  """, new
                                                                                       {
                                                                                           update.Name,
                                                                                           update.CharacterId,
                                                                                           update.AccountId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateDescriptionAsync(DescriptionUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character
                                                                                  set description = @Description
                                                                                  where id = @CharacterId
                                                                                  and account_id = @AccountId
                                                                                  """, new
                                                                                       {
                                                                                           Description = update.Description ?? string.Empty,
                                                                                           update.CharacterId,
                                                                                           update.AccountId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateHometownAsync(DescriptionUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character
                                                                                  set hometown = @Hometown
                                                                                  where id = @CharacterId
                                                                                  and account_id = @AccountId
                                                                                  """, new
                                                                                       {
                                                                                           Hometown = update.Hometown ?? string.Empty,
                                                                                           update.CharacterId,
                                                                                           update.AccountId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateXPAsync(DescriptionUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update characterstat
                                                                                  set current_xp = @XP
                                                                                  where character_id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.XP,
                                                                                           update.CharacterId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }
}