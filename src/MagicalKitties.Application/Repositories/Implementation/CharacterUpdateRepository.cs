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
    
    public async Task<bool> UpdateCunningAsync(AttributeUpdate update, string cunningId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update characterstat
                                                                                  set cunning = @Cunning
                                                                                  where character_id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.Cunning,
                                                                                           update.CharacterId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateCuteAsync(AttributeUpdate update, string cuteId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update characterstat
                                                                                  set cute = @Cute
                                                                                  where character_id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.Cute,
                                                                                           update.CharacterId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateFierceAsync(AttributeUpdate update, string fierceId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update characterstat
                                                                                  set fierce = @Fierce
                                                                                  where character_id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.Fierce,
                                                                                           update.CharacterId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateLevelAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update characterstat
                                                                                  set level = @Level
                                                                                  where character_id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.Level,
                                                                                           update.CharacterId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateFlawAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update characterflaw
                                                                                  set flaw_id = @NewId
                                                                                  where character_id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.FlawChange!.NewId,
                                                                                           update.CharacterId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateTalentAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update charactertalent
                                                                                  set talent_id = @NewId
                                                                                  where character_id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.TalentChange!.NewId,
                                                                                           update.CharacterId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public Task<bool> UpdateMagicalPowerAsync(AttributeUpdate update, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateOwiesAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update characterstat
                                                                                  set current_owies = @CurrentOwies
                                                                                  where character_id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.Owies,
                                                                                           update.CharacterId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateCurrentTreatsAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update characterstat
                                                                                  set current_treats = @CurrentTreats
                                                                                  where character_id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.CurrentTreats,
                                                                                           update.CharacterId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }
}