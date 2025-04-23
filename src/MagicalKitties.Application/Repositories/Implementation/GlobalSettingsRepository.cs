using System.Data;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.GlobalSettings;

namespace MagicalKitties.Application.Repositories.Implementation;

public class GlobalSettingsRepository : IGlobalSettingsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GlobalSettingsRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    public async Task<bool> CreateSetting(GlobalSetting setting, CancellationToken token = default)
    {
        using IDbConnection connection = await _connectionFactory.CreateConnectionAsync(token);
        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into globalsetting(id, name, value)
                                                                                  values(@id, @name, @value)
                                                                                  """, setting, cancellationToken: token));

        return result > 0;
    }

    public async Task<GlobalSetting?> GetSetting(string name, CancellationToken token = default)
    {
        using IDbConnection connection = await _connectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleOrDefaultAsyncWithRetry<GlobalSetting>(new CommandDefinition("""
                                                                                                        select * from globalsetting
                                                                                                        where name = @name
                                                                                                        """, new { name }, cancellationToken: token));
    }

    public async Task<IEnumerable<GlobalSetting>> GetAllAsync(GetAllGlobalSettingsOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _connectionFactory.CreateConnectionAsync(token);

        string orderClause = string.Empty;

        if (options.SortField is not null)
        {
            orderClause = $"order by {options.SortField} {(options.SortOrder == SortOrder.ascending ? "asc" : "desc")}";
        }

        IEnumerable<GlobalSetting> results = await connection.QueryAsyncWithRetry<GlobalSetting>(new CommandDefinition($"""
                                                                                                                        select * from globalsetting
                                                                                                                        where (@name is null or name like ('%' || @name || '%'))
                                                                                                                        {orderClause}
                                                                                                                        limit @pageSize
                                                                                                                        offset @pageOffset
                                                                                                                        """, new
                                                                                                                             {
                                                                                                                                 name = options.Name,
                                                                                                                                 pageSize = options.PageSize,
                                                                                                                                 pageOffset = (options.Page - 1) * options.PageSize
                                                                                                                             }, cancellationToken: token));

        return results;
    }

    public async Task<int> GetCountAsync(string name, CancellationToken token = default)
    {
        using IDbConnection connection = await _connectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleAsyncWithRetry<int>(new CommandDefinition("""
                                                                                     select count(id)
                                                                                     from globalsetting
                                                                                     where (@name is null or name like ('%' || @name || '%'))
                                                                                     """, new { name }, cancellationToken: token));
    }
}