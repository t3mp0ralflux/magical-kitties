﻿using System.Data;
using System.Text.Json;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Services.Implementation;

namespace MagicalKitties.Application.Repositories.Implementation;

public class CharacterRepository : ICharacterRepository
{
    private const string CharacterFields = "c.id, c.account_id as AccountId, c.username, c.name, c.created_utc as CreatedUtc, c.updated_utc as UpdatedUtc, c.deleted_utc as DeletedUtc, c.description, c.hometown, c.attributes";
    private const string StatsFields = "cs.level, cs.current_xp as CurrentXp, cs.max_owies as MaxOwies, cs.current_owies, cs.starting_treats as StartingTreats, cs.current_treats as CurrentTreats, cs.current_injuries as CurrentInjuries";

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

        int result = await connection.ExecuteAsync(new CommandDefinition("""
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
            result = await connection.ExecuteAsync(new CommandDefinition("""
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

    public async Task<Character?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        string shouldIncludeDeleted = includeDeleted ? string.Empty : "and c.deleted_utc is null";

        IEnumerable<Character> result = await connection.QueryAsync<Character>(new CommandDefinition($"""
                                                                                                      select {CharacterFields}, {StatsFields}
                                                                                                      from character c
                                                                                                      inner join characterstats cs on c.id = cs.character_id
                                                                                                      where c.id = @id
                                                                                                      {shouldIncludeDeleted}
                                                                                                      """, new { id }, cancellationToken: token));
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Character>> GetAllAsync(GetAllCharactersOptions options, CancellationToken token = default)
    {
//         using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
//
//         string orderClause = string.Empty;
//
//         if (options.SortField is not null)
//         {
//             orderClause = $"order by {options.SortField} {(options.SortOrder == SortOrder.ascending ? "asc" : "desc")}";
//         }
//
//         IEnumerable<Character> results = await connection.QueryAsync<Character, Characteristic, LevelInfo, Character>(new CommandDefinition($"""
//                                                                                                                                              select {CharacterFields}, {CharacteristicsFields}, {LevelInfoFields}
//                                                                                                                                              from character c
//                                                                                                                                                  left join characteristics ch on c.id = ch.character_id
//                                                                                                                                                  inner join characterlevel cl on cl.character_id = c.id
//                                                                                                                                                  inner join levelinfo li on cl.level_id = li.id
//                                                                                                                                              where c.account_id = @AccountId
//                                                                                                                                              and (@Name is null or lower(c.name) like ('%' || @Name || '%'))
//                                                                                                                                              and c.deleted_utc is null
//                                                                                                                                              {orderClause}
//                                                                                                                                              """, new
//                                                                                                                                                   {
//                                                                                                                                                       options.AccountId,
//                                                                                                                                                       Name = options.Name?.ToLowerInvariant()
//                                                                                                                                                   }, cancellationToken: token), (character, characteristics, levelInfo) =>
//                                                                                                                                                                                 {
//                                                                                                                                                                                     character.Characteristics = characteristics;
//                                                                                                                                                                                     character.Level = levelInfo.Level;
//                                                                                                                                                                                     return character;
//                                                                                                                                                                                 }, SplitFields);
//         return results;

        return []; // TODO: DON'T LEAVE IT THIS WAY.
    }

    public async Task<int> GetCountAsync(GetAllCharactersOptions options, CancellationToken token = default)
    {
        // using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        //
        // int result = await connection.QuerySingleAsync<int>(new CommandDefinition("""
        //                                                                           select count(c.id)
        //                                                                           from character c left join  characteristics ch on c.id = ch.character_id
        //                                                                           where c.account_id = @AccountId
        //                                                                           and (@Name is null or lower(c.name) like ('%' || @Name || '%'))
        //                                                                           and c.deleted_utc is null
        //                                                                           """, new
        //                                                                                {
        //                                                                                    options.AccountId,
        //                                                                                    Name = options.Name?.ToLowerInvariant()
        //                                                                                }, cancellationToken: token));
        //
        // return result;

        return 0; // TODO: DON'T LEAVE IT THIS WAY.
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        int result = await connection.QuerySingleAsync<int>(new CommandDefinition("""
                                                                                  select count(id)
                                                                                  from character
                                                                                  where id = @id
                                                                                  """, new { id }, cancellationToken: token));

        return result > 0;
    }

    public async Task<bool> UpdateAsync(Character character, CancellationToken token = default)
    {
        // using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        // using IDbTransaction transaction = connection.BeginTransaction();
        //
        // // note: don't update the dead
        // int result = await connection.ExecuteAsync(new CommandDefinition("""
        //                                                                  update character
        //                                                                  set name = @Name, updated_utc = @UpdatedUtc
        //                                                                  where id = @Id
        //                                                                  and deleted_utc is null
        //                                                                  """, new
        //                                                                       {
        //                                                                           character.Name,
        //                                                                           character.Id,
        //                                                                           character.UpdatedUtc
        //                                                                       }, cancellationToken: token));
        //
        // if (result > 0)
        // {
        //     await connection.ExecuteAsync(new CommandDefinition("""
        //                                                         update characteristics
        //                                                         set gender = @Gender, age = @Age, hair = @Hair, eyes = @Eyes, skin = @Skin, height = @Height, weight = @Weight, faith = @Faith
        //                                                         where character_id = @Id
        //                                                         """, new
        //                                                              {
        //                                                                  character.Characteristics.Gender,
        //                                                                  character.Characteristics.Age,
        //                                                                  character.Characteristics.Hair,
        //                                                                  character.Characteristics.Eyes,
        //                                                                  character.Characteristics.Skin,
        //                                                                  character.Characteristics.Height,
        //                                                                  character.Characteristics.Weight,
        //                                                                  character.Characteristics.Faith,
        //                                                                  character.Id
        //                                                              }, cancellationToken: token));
        // }
        //
        // transaction.Commit();
        //
        // return result > 0;

        return true; // TODO: DON'T LEAVE IT THIS WAY.
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsync(new CommandDefinition("""
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

    public async Task<bool> UpdateLevelAsync(Character character, Guid levelId, CancellationToken token)
    {
        // using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        // using IDbTransaction transaction = connection.BeginTransaction();
        //
        // int result = await connection.ExecuteAsync(new CommandDefinition("""
        //                                                                  update character
        //                                                                  set current_xp = @CurrentXp
        //                                                                  where id = @Id
        //                                                                  """, new { character.CurrentXp, character.Id }));
        //
        // if (result > 0)
        // {
        //     await connection.ExecuteAsync(new CommandDefinition("""
        //                                                         update characterlevel
        //                                                         set level_id = @levelId
        //                                                         where character_id = @Id
        //                                                         """, new { character.Id, levelId }, cancellationToken: token));
        // }
        //
        // transaction.Commit();
        //
        // return result > 0;

        return true; // TODO: DON'T LEAVE IT THIS WAY.
    }
}