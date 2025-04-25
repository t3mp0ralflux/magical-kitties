using System.Data;
using System.Text.Json;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Services.Implementation;

namespace MagicalKitties.Application.Repositories.Implementation;

public class CharacterRepository : ICharacterRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public CharacterRepository(IDbConnectionFactory dbConnectionFactory, IDateTimeProvider dateTimeProvider)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> CreateAsync(Character character, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into character(id, account_id, name, username, created_utc, updated_utc, deleted_utc, description, hometown, attributes)
                                                                                  values(@Id, @AccountId, @Name, @Username, @CreatedUtc, @UpdatedUtc, null, @Description, @Hometown, @Attributes)
                                                                                  """, new
                                                                                       {
                                                                                           character.Id,
                                                                                           character.AccountId,
                                                                                           character.Name,
                                                                                           character.Username,
                                                                                           CreatedUtc = _dateTimeProvider.GetUtcNow(),
                                                                                           UpdatedUtc = _dateTimeProvider.GetUtcNow(),
                                                                                           character.Description,
                                                                                           character.Hometown,
                                                                                           Attributes = new JsonParameter(JsonSerializer.Serialize(character.Attributes))
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into characterstat(id, character_id)
                                                                                  values(@Id, @CharacterId)
                                                                                  """, new
                                                                                       {
                                                                                           Id = Guid.NewGuid(),
                                                                                           CharacterId = character.Id
                                                                                       }, cancellationToken: token));
        }

        transaction.Commit();

        return result > 0;
    }

    public async Task<Character?> GetByIdAsync(Guid accountId, Guid id, bool includeDeleted = false, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        string shouldIncludeDeleted = includeDeleted ? string.Empty : "and c.deleted_utc is null";
        IEnumerable<Character> result = await connection.QueryAsyncWithRetry<Character, Flaw, List<Talent>>(new CommandDefinition($"""
                                                                                                               select c.id, c.account_id, c.username, c.name, c.created_utc, c.updated_utc, c.deleted_utc, c.description, c.hometown, c.attributes,
                                                                                                               cs.level, cs.current_xp, cs.max_owies, cs.current_owies, cs.starting_treats, cs.current_treats, cs.current_injuries,
                                                                                                               (select to_json(f.*)
                                                                                                                from flaw f
                                                                                                                right join characterflaw cf on f.id = cf.flaw_id
                                                                                                                where character_id = @id) as flaw,
                                                                                                               (select json_agg(t.*)
                                                                                                               from talent t
                                                                                                               inner join charactertalent ct on t.id = ct.talent_id
                                                                                                               where character_id = @id) as talents
                                                                                                               from character c
                                                                                                               inner join characterstat cs on c.id = cs.character_id
                                                                                                               where c.id = @id
                                                                                                               and c.account_id = @accountId
                                                                                                               {shouldIncludeDeleted}
                                                                                                               """, new { id, accountId }, cancellationToken: token), (character, flaw, talents) =>
                                                                                                                                                           {
                                                                                                                                                               character.Flaw = flaw;
                                                                                                                                                               character.Talents = talents;

                                                                                                                                                               return character;
                                                                                                                                                           }, "flaw,talents");
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Character>> GetAllAsync(GetAllCharactersOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        string orderClause = string.Empty;

        if (options.SortField is not null)
        {
            orderClause = $"order by {options.SortField} {(options.SortOrder == SortOrder.ascending ? "asc" : "desc")}";
        }

        IEnumerable<Character> results = await connection.QueryAsyncWithRetry<Character>(new CommandDefinition($"""
                                                                                                                select c.id, c.account_id, c.username, c.name, c.created_utc, c.updated_utc, c.deleted_utc, c.description, c.hometown, c.attributes,
                                                                                                                cs.level, cs.current_xp, cs.max_owies, cs.current_owies, cs.starting_treats, cs.current_treats, cs.current_injuries
                                                                                                                from character c
                                                                                                                inner join characterstat cs on c.id = cs.character_id
                                                                                                                where c.account_id = @AccountId
                                                                                                                and (@Name is null or lower(c.name) like ('%' || @Name || '%'))
                                                                                                                and c.deleted_utc is null
                                                                                                                {orderClause}
                                                                                                                """, new
                                                                                                                     {
                                                                                                                         options.AccountId,
                                                                                                                         Name = options.Name?.ToLowerInvariant()
                                                                                                                     }, cancellationToken: token));
        return results;
    }

    public async Task<int> GetCountAsync(GetAllCharactersOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        int result = await connection.QuerySingleAsyncWithRetry<int>(new CommandDefinition("""
                                                                                           select count(c.id)
                                                                                           from character c
                                                                                           where c.account_id = @AccountId
                                                                                           and (@Name is null or lower(c.name) like ('%' || @Name || '%'))
                                                                                           and c.deleted_utc is null
                                                                                           """, new
                                                                                                {
                                                                                                    options.AccountId,
                                                                                                    Name = options.Name?.ToLowerInvariant()
                                                                                                }, cancellationToken: token));

        return result;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        int result = await connection.QuerySingleAsyncWithRetry<int>(new CommandDefinition("""
                                                                                           select count(id)
                                                                                           from character
                                                                                           where id = @id
                                                                                           """, new { id }, cancellationToken: token));

        return result > 0;
    }

    public async Task<bool> UpdateAsync(Character character, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        // note: don't update the dead
        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character
                                                                                  set name = @Name, updated_utc = @UpdatedUtc
                                                                                  where id = @Id
                                                                                  and deleted_utc is null
                                                                                  """, new
                                                                                       {
                                                                                           character.Name,
                                                                                           character.Id,
                                                                                           character.UpdatedUtc
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character
                                                                                  set deleted_utc = @DeletedUtc
                                                                                  where id = @id
                                                                                  """, new
                                                                                       {
                                                                                           DeletedUtc = _dateTimeProvider.GetUtcNow(),
                                                                                           id
                                                                                       }, cancellationToken: token));
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateLevelAsync(LevelUpdate update, CancellationToken token)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update characterstat
                                                                                  set level = @Level
                                                                                  where id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.CharacterId,
                                                                                           update.Level
                                                                                       }));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateFlawAsync(FlawUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character
                                                                                  set flaw_id = @FlawId
                                                                                  where id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.CharacterId,
                                                                                           update.FlawId
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateTalentAsync(TalentUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character
                                                                                  set talent_id = @TalentId
                                                                                  where id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           update.CharacterId,
                                                                                           update.TalentId
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }
}