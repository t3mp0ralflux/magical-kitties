using System.Data;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.Humans.Updates;
using MagicalKitties.Application.Services;

namespace MagicalKitties.Application.Repositories.Implementation;

public class HumanRepository : IHumanRepository
{
    private const string HumanFields = "h.id, h.character_id, h.name, h.description";
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IDbConnectionFactory _dbConnectionFactory;


    public HumanRepository(IDbConnectionFactory dbConnectionFactory, IDateTimeProvider dateTimeProvider)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> CreateAsync(Human human, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into human(id, character_id, name, description, deleted_utc)
                                                                                  values(@Id, @CharacterId, @Name, @Description, null)
                                                                                  """, new
                                                                                       {
                                                                                           human.Id,
                                                                                           human.CharacterId,
                                                                                           human.Name,
                                                                                           human.Description
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character
                                                                                  set updated_utc = @Now
                                                                                  where id = @CharacterId
                                                                                  """, new
                                                                                       {
                                                                                           Now = _dateTimeProvider.GetUtcNow(),
                                                                                           human.CharacterId
                                                                                       }, cancellationToken: token));
        }

        transaction.Commit();

        return result > 0;
    }
    
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QuerySingleAsyncWithRetry<bool>(new CommandDefinition("""
                                                                                      select exists(select 1
                                                                                      from human
                                                                                      where id = @id)
                                                                                      """, new { id }, cancellationToken: token));
    }

    public async Task<Human?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        string shouldIncludeDeleted = includeDeleted ? string.Empty : "and h.deleted_utc is null";
        IEnumerable<Human> result = await connection.QueryAsyncWithRetry<Human, List<Problem>>(new CommandDefinition($"""
                                                                                                                      select {HumanFields},
                                                                                                                      (select json_agg(p.*)
                                                                                                                      from problem p
                                                                                                                      where human_id = @id) as problems
                                                                                                                      from human h
                                                                                                                      where h.id = @id
                                                                                                                      {shouldIncludeDeleted}
                                                                                                                      """, new
                                                                                                                           {
                                                                                                                               id
                                                                                                                           }, cancellationToken: token), (human, problems) =>
                                                                                                                                                         {
                                                                                                                                                             human.Problems = problems;

                                                                                                                                                             return human;
                                                                                                                                                         }, "problems");

        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Human>> GetAllAsync(GetAllHumansOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        string orderClause = string.Empty;

        if (options.SortField is not null)
        {
            orderClause = $"order by {options.SortField} {(options.SortOrder == SortOrder.ascending ? "asc" : "desc")}";
        }

        IEnumerable<Human> result = await connection.QueryAsyncWithRetry<Human>(new CommandDefinition($"""
                                                                                                       select {HumanFields}
                                                                                                       from human h
                                                                                                       where h.character_id = @CharacterId
                                                                                                       and (@Name is null or lower(h.name) like ('%' || @Name || '%'))
                                                                                                       and h.deleted_utc is null
                                                                                                       {orderClause}
                                                                                                       limit @pageSize
                                                                                                       offset @pageOffset
                                                                                                       """, new
                                                                                                            {
                                                                                                                options.CharacterId,
                                                                                                                Name = options.Name?.ToLowerInvariant(),
                                                                                                                pageSize = options.PageSize,
                                                                                                                pageOffset = (options.Page - 1) * options.PageSize
                                                                                                            }));

        return result;
    }

    public async Task<int> GetCountAsync(GetAllHumansOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        int result = await connection.QuerySingleAsyncWithRetry<int>(new CommandDefinition("""
                                                                                           select count(h.id)
                                                                                           from human h
                                                                                           where h.character_id = @CharacterId
                                                                                           and (@Name is null or lower(h.name) like ('%' || @Name || '%'))
                                                                                           and h.deleted_utc is null
                                                                                           """, new
                                                                                                {
                                                                                                    options.CharacterId,
                                                                                                    Name = options.Name?.ToLowerInvariant()
                                                                                                }));

        return result;
    }

    public async Task<bool> UpdateDescriptionAsync(DescriptionUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update human
                                                                                  set description = @Description
                                                                                  where id = @HumanId
                                                                                  """, new
                                                                                       {
                                                                                           update.Description,
                                                                                           update.HumanId
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateNameAsync(DescriptionUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update human
                                                                                  set name = @Name
                                                                                  where id = @HumanId
                                                                                  """, new
                                                                                       {
                                                                                           update.Name,
                                                                                           update.HumanId
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update human
                                                                                  set deleted_utc = @Now
                                                                                  where id = @id
                                                                                  """, new
                                                                                       {
                                                                                           Now = _dateTimeProvider.GetUtcNow(),
                                                                                           id
                                                                                       }, cancellationToken: token));

        if (result > 0)
        {
            result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update problem
                                                                                  set deleted_utc = @Now
                                                                                  where human_id = @id
                                                                                  """, new
                                                                                       {
                                                                                           Now = _dateTimeProvider.GetUtcNow(),
                                                                                           id
                                                                                       }, cancellationToken: token));
        }

        if (result > 0)
        {
            result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character c
                                                                                  set updated_utc = @Now
                                                                                  where c.id = (select h.character_id from human h where h.id = @id)
                                                                                  """, new
                                                                                       {
                                                                                           Now = _dateTimeProvider.GetUtcNow(),
                                                                                           id
                                                                                       }, cancellationToken: token));
        }

        transaction.Commit();

        return result > 0;
    }
}