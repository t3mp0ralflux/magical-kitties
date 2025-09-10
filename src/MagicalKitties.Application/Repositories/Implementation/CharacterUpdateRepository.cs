using System.Data;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Talents;
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

    public async Task<bool> UpdateXPAsync(AttributeUpdate update, CancellationToken token = default)
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
                                                                                           CharacterId = update.Character.Id
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
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
                                                                                           CharacterId = update.Character.Id
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
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
                                                                                           CharacterId = update.Character.Id
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
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
                                                                                           CharacterId = update.Character.Id
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
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
                                                                                           CharacterId = update.Character.Id
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
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
                                                                                           CharacterId = update.Character.Id,
                                                                                           FlawId = update.FlawChange!.NewId
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
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
                                                                                           CharacterId = update.Character.Id
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
        }

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> CreateTalentAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into charactertalent(id, character_id, talent_id, is_primary)
                                                                                  values(@Id, @CharacterId, @TalentId, @IsPrimary)
                                                                                  """, new
                                                                                       {
                                                                                           Id = Guid.NewGuid(),
                                                                                           CharacterId = update.Character.Id,
                                                                                           TalentId = update.TalentChange!.NewId,
                                                                                           update.TalentChange.IsPrimary
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
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
                                                                                  and is_primary = @IsPrimary
                                                                                  """, new
                                                                                       {
                                                                                           update.TalentChange!.NewId,
                                                                                           CharacterId = update.Character.Id,
                                                                                           update.TalentChange!.PreviousId,
                                                                                           update.TalentChange.IsPrimary
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
        }

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> RemoveTalentAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  delete from charactertalent 
                                                                                  where character_id = @CharacterId
                                                                                  and talent_id = @NewId
                                                                                  and is_primary = @IsPrimary
                                                                                  """, new
                                                                                           {
                                                                                               CharacterId = update.Character.Id,
                                                                                               update.TalentChange!.NewId,
                                                                                               update.TalentChange.IsPrimary
                                                                                           }, cancellationToken: token));
        
        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
        }

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> CreateMagicalPowerAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into charactermagicalpower(id, character_id, magical_power_id, is_primary)
                                                                                  values(@Id, @CharacterId, @MagicalPowerId, @IsPrimary)
                                                                                  """, new
                                                                                       {
                                                                                           Id = Guid.NewGuid(),
                                                                                           CharacterId = update.Character.Id,
                                                                                           MagicalPowerId = update.MagicalPowerChange!.NewId,
                                                                                           update.MagicalPowerChange.IsPrimary
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
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
                                                                                  and is_primary = @IsPrimary
                                                                                  """, new
                                                                                       {
                                                                                           update.MagicalPowerChange!.NewId,
                                                                                           CharacterId = update.Character.Id,
                                                                                           update.MagicalPowerChange!.PreviousId,
                                                                                           update.MagicalPowerChange.IsPrimary
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
        }

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> RemoveMagicalPowerAsync(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  delete from charactermagicalpower
                                                                                  where character_id = @CharacterId
                                                                                  and magical_power_id = @NewId
                                                                                  and is_primary = @IsPrimary
                                                                                  """, new
                                                                                       {
                                                                                           update.MagicalPowerChange!.NewId,
                                                                                           CharacterId = update.Character.Id,
                                                                                           update.MagicalPowerChange.IsPrimary
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
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
                                                                                           CharacterId = update.Character.Id
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
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
                                                                                           CharacterId = update.Character.Id
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
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
                                                                                           CharacterId = update.Character.Id
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
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
                                                                                           CharacterId = update.Character.Id
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await UpdateCharacterUpdateUtcAsync(update.Character.Id, connection, token);
        }

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ClearUpgradesOnCharacter(AttributeUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character
                                                                                  set upgrades = null
                                                                                  where id = @Id
                                                                                  """, new
                                                                                       {
                                                                                          update.Character.Id
                                                                                       }, cancellationToken: token));

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