using System.Data;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.Talents;

namespace MagicalKitties.Application.Repositories.Implementation;

public class TalentRepository : ITalentRepository
{
    private readonly IDbConnectionFactory _dbonConnectionFactory;

    public TalentRepository(IDbConnectionFactory dbonConnectionFactory)
    {
        _dbonConnectionFactory = dbonConnectionFactory;
    }

    public async Task<bool> CreateAsync(Talent talent, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into talent(id, name, description, is_custom)
                                                                                  values (@Id, @Name, @Description, @IsCustom)
                                                                                  """, new
                                                                                       {
                                                                                           talent.Id,
                                                                                           talent.Name,
                                                                                           talent.Description,
                                                                                           talent.IsCustom
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<Talent?> GetByIdAsync(int id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);

        Talent? result = await connection.QuerySingleOrDefaultAsyncWithRetry<Talent>(new CommandDefinition("""
                                                                                                           select id, name, description, is_custom
                                                                                                           from talent
                                                                                                           where id = @id
                                                                                                           """, new { id }, cancellationToken: token));

        return result;
    }

    public async Task<bool> ExistsByIdAsync(int id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);

        int result = await connection.QuerySingleOrDefaultAsyncWithRetry<int>(new CommandDefinition("""
                                                                                                    select count(id)
                                                                                                    from talent
                                                                                                    where id = @id
                                                                                                    """, new { id }, cancellationToken: token));

        return result > 0;
    }

    public async Task<IEnumerable<Talent>> GetAllAsync(GetAllTalentsOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);

        string orderClause = string.Empty;

        if (options.SortField is not null)
        {
            orderClause = $"order by {options.SortField} {(options.SortOrder == SortOrder.ascending ? "asc" : "desc")}";
        }

        IEnumerable<Talent> results = await connection.QueryAsyncWithRetry<Talent>(new CommandDefinition($"""
                                                                                                          select id, name, description, is_custom
                                                                                                          from talent
                                                                                                          {orderClause}
                                                                                                          """, new
                                                                                                               {
                                                                                                                   options
                                                                                                               }, cancellationToken: token));

        return results;
    }

    public async Task<int> GetCountAsync(GetAllTalentsOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);

        string orderClause = string.Empty;

        if (options.SortField is not null)
        {
            orderClause = $"order by {options.SortField} {(options.SortOrder == SortOrder.ascending ? "asc" : "desc")}";
        }

        int result = await connection.QuerySingleAsyncWithRetry<int>(new CommandDefinition($"""
                                                                                            select count(id)
                                                                                            from talent
                                                                                            {orderClause}
                                                                                            """, new
                                                                                                 {
                                                                                                     options
                                                                                                 }, cancellationToken: token));

        return result;
    }

    public async Task<bool> UpdateAsync(Talent talent, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update talent
                                                                                  set name = @Name, description = @Description
                                                                                  where id = @Id
                                                                                  """, new
                                                                                       {
                                                                                           talent.Name,
                                                                                           talent.Description,
                                                                                           talent.Id
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  delete from talent
                                                                                  where id = @id
                                                                                  """, new { id }, cancellationToken: token));
        transaction.Commit();

        return result > 0;
    }
}