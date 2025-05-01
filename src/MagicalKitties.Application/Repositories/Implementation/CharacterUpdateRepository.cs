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
    
    public async Task<bool> UpdateCunningAsync(AttributeUpdate update, CancellationToken token = default)
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

    public async Task<bool> UpdateCuteAsync(AttributeUpdate update, CancellationToken token = default)
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

    public async Task<bool> UpdateFierceAsync(AttributeUpdate update, CancellationToken token = default)
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

    public async Task<bool> CreateFlawAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into characterflaw(id, character_id, flaw_id)
                                                                                  values(@Id, @CharacterId, @FlawId)
                                                                                  """, new
                                                                                       {
                                                                                           Id = Guid.NewGuid(),
                                                                                           update.CharacterId,
                                                                                           FlawId = update.FlawChange!.NewId
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

    public async Task<bool> CreateTalentAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into charactertalent(id, character_id, talent_id)
                                                                                  values(@Id, @CharacterId, @TalentId)
                                                                                  """, new
                                                                                       {
                                                                                           Id = Guid.NewGuid(),
                                                                                           update.CharacterId,
                                                                                           TalentId = update.TalentChange!.NewId
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
                                                                                  and talent_id = @PreviousId
                                                                                  """, new
                                                                                       {
                                                                                           update.TalentChange!.NewId,
                                                                                           update.CharacterId,
                                                                                           update.TalentChange!.PreviousId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> CreateMagicalPowerAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into charactermagicalpower(id, character_id, magical_power_id)
                                                                                  values(@Id, @CharacterId, @MagicalPowerId)
                                                                                  """, new
                                                                                       {
                                                                                           Id = Guid.NewGuid(),
                                                                                           update.CharacterId,
                                                                                           MagicalPowerId = update.MagicalPowerChange!.NewId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateMagicalPowerAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update charactermagicalpower
                                                                                  set magical_power_id = @NewId
                                                                                  where character_id = @CharacterId
                                                                                  and magical_power_id = @PreviousId
                                                                                  """, new
                                                                                       {
                                                                                           update.MagicalPowerChange!.NewId,
                                                                                           update.CharacterId,
                                                                                           update.MagicalPowerChange!.PreviousId
                                                                                           
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
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
                                                                                           update.CurrentOwies,
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

    public async Task<bool> UpdateCurrentInjuriesAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update characterstat
                                                                                  set current_injuries = @CurrentInjuries
                                                                                  where character_id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.CurrentInjuries,
                                                                                           update.CharacterId
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }
}