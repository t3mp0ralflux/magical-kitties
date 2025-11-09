using System.Data;
using System.Text;
using System.Text.Json;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Services;

namespace MagicalKitties.Application.Repositories.Implementation;

public class CharacterRepository : ICharacterRepository
{
    private const string CharacterFields = "c.id, c.account_id, c.username, c.name, c.created_utc, c.updated_utc, c.deleted_utc, c.description, c.hometown, json(c.upgrades) as upgrades";
    private const string CharacterStatFields = "cs.level, cs.current_xp, cs.max_owies, cs.current_owies, cs.starting_treats, cs.current_treats, cs.current_injuries, cs.cute, cs.cunning, cs.fierce";
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
                                                                                  insert into character(id, account_id, name, username, created_utc, updated_utc, deleted_utc, description, hometown)
                                                                                  values(@Id, @AccountId, @Name, @Username, @CreatedUtc, @UpdatedUtc, null, @Description, @Hometown)
                                                                                  """, new
                                                                                       {
                                                                                           character.Id,
                                                                                           character.AccountId,
                                                                                           character.Name,
                                                                                           character.Username,
                                                                                           CreatedUtc = _dateTimeProvider.GetUtcNow(),
                                                                                           UpdatedUtc = _dateTimeProvider.GetUtcNow(),
                                                                                           character.Description,
                                                                                           character.Hometown
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

    public async Task<Character?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        string shouldIncludeDeleted = includeDeleted ? string.Empty : "and c.deleted_utc is null";
        IEnumerable<Character> result = await connection.QueryAsyncWithRetry<Character, Flaw, List<Talent>, List<MagicalPower>, List<Human>>(new CommandDefinition($"""
                                                                                                                                                                    select {CharacterFields}, {CharacterStatFields},
                                                                                                                                                                    (select to_json(f.*)
                                                                                                                                                                     from flaw f
                                                                                                                                                                     inner join characterflaw cf on f.id = cf.flaw_id
                                                                                                                                                                     where character_id = @id) as flaw,
                                                                                                                                                                    (with talent_info as 
                                                                                                                                                                        (select t.*, ct.is_primary from talent t
                                                                                                                                                                    inner join charactertalent ct on t.id = ct.talent_id
                                                                                                                                                                    where character_id = @id)
                                                                                                                                                                    select json_agg(talent_info.*)
                                                                                                                                                                    from talent_info
                                                                                                                                                                    ) as talents,
                                                                                                                                                                    (with mp_info as 
                                                                                                                                                                        (select mp.*, cmp.is_primary from magicalpower mp
                                                                                                                                                                    inner join charactermagicalpower cmp on mp.id = cmp.magical_power_id
                                                                                                                                                                    where character_id = @id)
                                                                                                                                                                    select json_agg(mp_info.*)
                                                                                                                                                                    from mp_info
                                                                                                                                                                     ) as magicalpowers,
                                                                                                                                                                    (select json_agg(
                                                                                                                                                                                     json_build_object(
                                                                                                                                                                                        'id', h.id,
                                                                                                                                                                                        'character_id', h.character_id,
                                                                                                                                                                                        'name', h.name,
                                                                                                                                                                                        'description', h.description,
                                                                                                                                                                                        'deleted_utc', h.deleted_utc,
                                                                                                                                                                                        'problems', coalesce(problems, '[]'::json)
                                                                                                                                                                                     )
                                                                                                                                                                            )
                                                                                                                                                                    from human h
                                                                                                                                                                    left join (
                                                                                                                                                                        select
                                                                                                                                                                            p.human_id,
                                                                                                                                                                            json_agg (
                                                                                                                                                                                    json_build_object(
                                                                                                                                                                                            'id', p.id,
                                                                                                                                                                                            'human_id', p.human_id,
                                                                                                                                                                                            'source', p.source,
                                                                                                                                                                                            'solved', p.solved,
                                                                                                                                                                                            'emotion', p.emotion,
                                                                                                                                                                                            'rank', p.rank,
                                                                                                                                                                                            'deleted_utc', p.deleted_utc
                                                                                                                                                                                    )
                                                                                                                                                                            ) problems
                                                                                                                                                                        from problem p
                                                                                                                                                                        group by p.human_id
                                                                                                                                                                    ) p on h.id = p.human_id
                                                                                                                                                                    where h.character_id = @id and h.deleted_utc is null) humans
                                                                                                                                                                    from character c
                                                                                                                                                                    inner join characterstat cs on c.id = cs.character_id
                                                                                                                                                                    where c.id = @id
                                                                                                                                                                    {shouldIncludeDeleted}
                                                                                                                                                                    """, new { id }, cancellationToken: cancellationToken), (character, flaw, talents, magicalPowers, humans) =>
                                                                                                                                                                                                                                       {
                                                                                                                                                                                                                                           character.Flaw = string.IsNullOrWhiteSpace(flaw?.Name) ? null : flaw;
                                                                                                                                                                                                                                           character.Talents = talents;
                                                                                                                                                                                                                                           character.MagicalPowers = magicalPowers;
                                                                                                                                                                                                                                           character.Humans = humans;

                                                                                                                                                                                                                                           return character;
                                                                                                                                                                                                                                       }, "flaw,talents,magicalpowers,humans");
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Character>> GetAllAsync(GetAllCharactersOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        string orderClause = options.SortField is not null 
            ? $"order by {options.SortField} {(options.SortOrder == SortOrder.ascending ? "asc" : "desc")}" 
            : "order by name asc";

        IEnumerable<Character> results = await connection.QueryAsyncWithRetry<Character>(new CommandDefinition($"""
                                                                                                                select {CharacterFields}, {CharacterStatFields}
                                                                                                                from character c
                                                                                                                inner join characterstat cs on c.id = cs.character_id
                                                                                                                where c.account_id = @AccountId
                                                                                                                and (@SearchInput is null or lower(c.name) like ('%' || @SearchInput || '%') or to_char(cs.level, 'FM99MI') = @SearchInput)
                                                                                                                and c.deleted_utc is null
                                                                                                                {orderClause}
                                                                                                                limit @pageSize
                                                                                                                offset @pageOffset
                                                                                                                """, new
                                                                                                                     {
                                                                                                                         options.AccountId,
                                                                                                                         SearchInput = options.SearchInput?.ToLowerInvariant(),
                                                                                                                         pageSize = options.PageSize,
                                                                                                                         pageOffset = (options.Page - 1) * options.PageSize
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
                                                                                                    Name = options.SearchInput?.ToLowerInvariant()
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

    public async Task<bool> CopyAsync(Character existingCharacter, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();
        
        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into character(id, account_id, name, username, created_utc, updated_utc, deleted_utc, description, hometown, upgrades)
                                                                                  values(@Id, @AccountId, @Name, @Username, @CreatedUtc, @UpdatedUtc, null, @Description, @Hometown, @Upgrades)
                                                                                  """, new
                                                                                       {
                                                                                           existingCharacter.Id,
                                                                                           existingCharacter.AccountId,
                                                                                           existingCharacter.Name,
                                                                                           existingCharacter.Username,
                                                                                           CreatedUtc = _dateTimeProvider.GetUtcNow(),
                                                                                           UpdatedUtc = _dateTimeProvider.GetUtcNow(),
                                                                                           existingCharacter.Description,
                                                                                           existingCharacter.Hometown,
                                                                                           Upgrades = new JsonParameter(JsonSerializer.Serialize(existingCharacter.Upgrades)),
                                                                                       }, cancellationToken: token));

        if (result < 0)
        {
            return false;
        }
        
        result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                              insert into characterstat(id, character_id, level, current_xp, max_owies, current_owies, starting_treats, current_treats, current_injuries, cute, cunning, fierce, incapacitated)
                                                                              values (@Id, @CharacterId, @Level, @CurrentXp, @MaxOwies, @CurrentOwies, @StartingTreats, @CurrentTreats, @CurrentInjuries, @Cute, @Cunning, @Fierce, @Incapacitated)
                                                                              """, new
                                                                                   {
                                                                                       Id = Guid.NewGuid(),
                                                                                       CharacterId = existingCharacter.Id,
                                                                                       existingCharacter.Level,
                                                                                       existingCharacter.CurrentXp,
                                                                                       existingCharacter.MaxOwies,
                                                                                       existingCharacter.CurrentOwies,
                                                                                       existingCharacter.StartingTreats,
                                                                                       existingCharacter.CurrentTreats,
                                                                                       existingCharacter.CurrentInjuries,
                                                                                       existingCharacter.Cute,
                                                                                       existingCharacter.Cunning,
                                                                                       existingCharacter.Fierce,
                                                                                       existingCharacter.Incapacitated
                                                                                   }, cancellationToken: token));
        
        if (result < 0)
        {
            return false;
        }
        
        if(existingCharacter.Flaw is not null)
        {
            result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into characterflaw(id, character_id, flaw_id)
                                                                                  values(@Id, @CharacterId, @FlawId)
                                                                                  """, new
                                                                                       {
                                                                                           Id = Guid.NewGuid(),
                                                                                           CharacterId = existingCharacter.Id,
                                                                                           FlawId = existingCharacter.Flaw?.Id
                                                                                       }, cancellationToken: token));
            if (result < 0)
            {
                return false;
            }
        }
        
        foreach (Talent existingCharacterTalent in existingCharacter.Talents)
        {
            result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into charactertalent(id, character_id, talent_id, is_primary)
                                                                                  values (@Id, @CharacterId, @TalentId, @IsPrimary)
                                                                                  """, new
                                                                                       {
                                                                                           Id = Guid.NewGuid(),
                                                                                           CharacterId = existingCharacter.Id,
                                                                                           TalentId = existingCharacterTalent.Id,
                                                                                           existingCharacterTalent.IsPrimary
                                                                                       }, cancellationToken: token));

            if (result < 0)
            {
                return false;
            }
        }
        
        foreach (MagicalPower existingCharacterMagicalPower in existingCharacter.MagicalPowers)
        {
            result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into charactermagicalpower(id, character_id, magical_power_id, is_primary)
                                                                                  values (@Id, @CharacterId, @MagicalPowerId, @IsPrimary)
                                                                                  """, new
                                                                                       {
                                                                                           Id = Guid.NewGuid(),
                                                                                           CharacterId = existingCharacter.Id,
                                                                                           MagicalPowerId = existingCharacterMagicalPower.Id,
                                                                                           existingCharacterMagicalPower.IsPrimary
                                                                                       }, cancellationToken: token));

            if (result < 0)
            {
                return false;
            }
        }
        
        foreach (Human existingCharacterHuman in existingCharacter.Humans)
        {
            result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into human(id, character_id, name, description, deleted_utc)
                                                                                  values (@Id, @CharacterId, @Name, @Description, @DeletedUtc)
                                                                                  """, new
                                                                                       {
                                                                                           existingCharacterHuman.Id,
                                                                                           CharacterId = existingCharacter.Id,
                                                                                           existingCharacterHuman.Name,
                                                                                           existingCharacterHuman.Description,
                                                                                           existingCharacterHuman.DeletedUtc
                                                                                       }, cancellationToken: token));
            
            if (result < 0)
            {
                return false;
            }
            
            foreach (Problem problem in existingCharacterHuman.Problems)
            {
                result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                      insert into problem(id, human_id, source, emotion, rank, solved, deleted_utc)
                                                                                      values (@Id, @HumanId, @Source, @Emotion, @Rank, @Solved, @DeletedUtc)
                                                                                      """, new
                                                                                           {
                                                                                               problem.Id,
                                                                                               problem.HumanId,
                                                                                               problem.Source,
                                                                                               problem.Emotion,
                                                                                               problem.Rank,
                                                                                               problem.Solved,
                                                                                               problem.DeletedUtc
                                                                                           }, cancellationToken: token));
            
                if (result < 0)
                {
                    return false;
                }
            }
        }
        
        transaction.Commit();

        return result > 0;
    }
}