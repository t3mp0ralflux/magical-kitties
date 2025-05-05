using System.Data;
using System.Text.Json;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.MagicalPowers;

namespace MagicalKitties.Application.Repositories.Implementation;

public class MagicalPowerRepository : IMagicalPowerRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MagicalPowerRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(MagicalPower magicalpower, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into magicalpower(id, name, description, is_custom, bonusfeatures)
                                                                                  values (@Id, @Name, @Description, @IsCustom, @BonusFeatures)
                                                                                  """, new
                                                                                       {
                                                                                           magicalpower.Id,
                                                                                           magicalpower.Name,
                                                                                           magicalpower.Description,
                                                                                           magicalpower.IsCustom,
                                                                                           BonusFeatures = new JsonParameter(JsonSerializer.Serialize(magicalpower.BonusFeatures))
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<MagicalPower?> GetByIdAsync(int id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        MagicalPower? result = await connection.QuerySingleOrDefaultAsyncWithRetry<MagicalPower>(new CommandDefinition("""
                                                                                                                       select id, name, description, is_custom as IsCustom, bonusfeatures
                                                                                                                       from magicalpower
                                                                                                                       where id = @id
                                                                                                                       """, new { id }, cancellationToken: token));

        return result;
    }

    public async Task<bool> ExistsByIdAsync(int id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QuerySingleOrDefaultAsyncWithRetry<bool>(new CommandDefinition("""
                                                                                                    select exists(select 1
                                                                                                    from magicalpower
                                                                                                    where id = @id)
                                                                                                    """, new { id }, cancellationToken: token));
    }

    public async Task<IEnumerable<MagicalPower>> GetAllAsync(GetAllMagicalPowersOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        string orderClause = string.Empty;

        if (options.SortField is not null)
        {
            orderClause = $"order by {options.SortField} {(options.SortOrder == SortOrder.ascending ? "asc" : "desc")}";
        }

        IEnumerable<MagicalPower> results = await connection.QueryAsyncWithRetry<MagicalPower>(new CommandDefinition($"""
                                                                                                                      select id, name, description, is_custom as IsCustom, bonusfeatures
                                                                                                                      from magicalpower
                                                                                                                      {orderClause}
                                                                                                                      """, new
                                                                                                                           {
                                                                                                                               options
                                                                                                                           }, cancellationToken: token));

        return results;
    }

    public async Task<int> GetCountAsync(GetAllMagicalPowersOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        string orderClause = string.Empty;

        if (options.SortField is not null)
        {
            orderClause = $"order by {options.SortField} {(options.SortOrder == SortOrder.ascending ? "asc" : "desc")}";
        }

        int result = await connection.QuerySingleAsyncWithRetry<int>(new CommandDefinition($"""
                                                                                            select count(id)
                                                                                            from magicalpower
                                                                                            {orderClause}
                                                                                            """, new
                                                                                                 {
                                                                                                     options
                                                                                                 }, cancellationToken: token));

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  delete from magicalpower
                                                                                  where id = @id
                                                                                  """, new { id }, cancellationToken: token));
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateAsync(MagicalPower magicalPower, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update magicalpower
                                                                                  set name = @Name, description = @Description, bonusfeatures = @BonusFeatures
                                                                                  where id = @Id
                                                                                  """, new
                                                                                       {
                                                                                           magicalPower.Name,
                                                                                           magicalPower.Description,
                                                                                           magicalPower.Id,
                                                                                           BonusFeatures = new JsonParameter(JsonSerializer.Serialize(magicalPower.BonusFeatures))
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }
}