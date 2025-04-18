using System.Data;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Flaws;

namespace MagicalKitties.Application.Repositories.Implementation;

public class FlawRepository : IFlawRepository
{
    private readonly IDbConnectionFactory _dbonConnectionFactory;

    public FlawRepository(IDbConnectionFactory dbonConnectionFactory)
    {
        _dbonConnectionFactory = dbonConnectionFactory;
    }

    public async Task<bool> CreateAsync(Endowment flaw, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into flaw(id, name, description, is_custom)
                                                                         values (@Id, @Name, @Description, @IsCustom)
                                                                         """, new
                                                                              {
                                                                                  flaw.Id,
                                                                                  flaw.Name,
                                                                                  flaw.Description,
                                                                                  flaw.IsCustom
                                                                              }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<Endowment?> GetByIdAsync(int id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);

        Endowment? result = await connection.QuerySingleOrDefaultAsync<Endowment>(new CommandDefinition("""
                                                                                                        select id, name, description, is_custom as IsCustom
                                                                                                        from flaw
                                                                                                        where id = @id
                                                                                                        """, new { id }, cancellationToken: token));
        
        return result;
    }

    public async Task<bool> ExistsByIdAsync(int id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);

        int result = await connection.QuerySingleOrDefaultAsync<int>(new CommandDefinition("""
                                                                                                        select count(id)
                                                                                                        from flaw
                                                                                                        where id = @id
                                                                                                        """, new { id }, cancellationToken: token));
        
        return result > 0;
    }

    public async Task<IEnumerable<Endowment>> GetAllAsync(GetAllFlawsOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);

        string orderClause = string.Empty;

        if (options.SortField is not null)
        {
            orderClause = $"order by {options.SortField} {(options.SortOrder == SortOrder.ascending ? "asc" : "desc")}";
        }

        IEnumerable<Endowment> results = await connection.QueryAsync<Endowment>(new CommandDefinition($"""
                                                                                                       select id, name, description, is_custom as IsCustom
                                                                                                       from flaw
                                                                                                       {orderClause}
                                                                                                       """, new
                                                                                                            {
                                                                                                                options
                                                                                                            }, cancellationToken: token));

        return results;
    }

    public async Task<int> GetCountAsync(GetAllFlawsOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);

        string orderClause = string.Empty;

        if (options.SortField is not null)
        {
            orderClause = $"order by {options.SortField} {(options.SortOrder == SortOrder.ascending ? "asc" : "desc")}";
        }

        int result = await connection.QuerySingleAsync<int>(new CommandDefinition($"""
                                                                                    select count(id)
                                                                                    from flaw
                                                                                    {orderClause}
                                                                                    """, new
                                                                                         {
                                                                                             options
                                                                                         }, cancellationToken: token));

        return result;
    }

    public async Task<bool> UpdateAsync(Endowment flaw, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         update flaw
                                                                         set name = @Name, description = @Description
                                                                         where id = @Id
                                                                         """, new
                                                                              {
                                                                                  flaw.Name,
                                                                                  flaw.Description,
                                                                                  flaw.Id
                                                                              }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbonConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         delete from flaw
                                                                         where id = @id
                                                                         """, new { id }, cancellationToken: token));
        transaction.Commit();

        return result > 0;
    }
}