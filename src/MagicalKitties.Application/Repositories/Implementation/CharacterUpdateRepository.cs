using System.Data;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Services;

namespace MagicalKitties.Application.Repositories.Implementation;

public class CharacterUpdateRepository : ICharacterUpdateRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public CharacterUpdateRepository(IDbConnectionFactory dbConnectionFactory, IDateTimeProvider dateTimeProvider)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> UpdateNameAsync(DescriptionUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character
                                                                                  set name = @Name, updated_utc = @Now
                                                                                  where id = @CharacterId
                                                                                  and account_id = @AccountId
                                                                                  """, new
                                                                                       {
                                                                                           update.Name,
                                                                                           update.CharacterId,
                                                                                           update.AccountId,
                                                                                           Now = _dateTimeProvider.GetUtcNow()
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
                                                                                  set description = @Description, updated_utc = @Now
                                                                                  where id = @CharacterId
                                                                                  and account_id = @AccountId
                                                                                  """, new
                                                                                       {
                                                                                           Description = update.Description ?? string.Empty,
                                                                                           update.CharacterId,
                                                                                           update.AccountId,
                                                                                           Now = _dateTimeProvider.GetUtcNow()
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
                                                                                  set hometown = @Hometown, updated_utc = @Now
                                                                                  where id = @CharacterId
                                                                                  and account_id = @AccountId
                                                                                  """, new
                                                                                       {
                                                                                           Hometown = update.Hometown ?? string.Empty,
                                                                                           update.CharacterId,
                                                                                           update.AccountId,
                                                                                           Now = _dateTimeProvider.GetUtcNow()
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

        if (result > 0)
        {
            result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character
                                                                                  set updated_utc = @Now
                                                                                  where id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.CharacterId,
                                                                                           Now = _dateTimeProvider.GetUtcNow()
                                                                                       }, cancellationToken: token));
        }

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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateCurrentOwiesAsync(AttributeUpdate update, CancellationToken token = default)
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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

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

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateIncapacitatedStatus(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update characterstat
                                                                                  set incapacitated = @Incapacitated
                                                                                  where character_id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.Incapacitated,
                                                                                           update.CharacterId
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.CharacterId, connection, token);
        }

        transaction.Commit();

        return result > 0;
    }

    private async Task<int> UpdateCharacterUpdateUtcAsync(Guid characterId, IDbConnection connection, CancellationToken token)
    {
        return await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                            update character
                                                                            set updated_utc = @Now
                                                                            where id = @characterId
                                                                            """, new
                                                                                 {
                                                                                     characterId,
                                                                                     Now = _dateTimeProvider.GetUtcNow()
                                                                                 }, cancellationToken: token));
    }
}