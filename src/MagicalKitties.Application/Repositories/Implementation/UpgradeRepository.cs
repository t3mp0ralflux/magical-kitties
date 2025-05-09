using System.Data;
using System.Text.Json;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Repositories.Implementation;

public class UpgradeRepository : IUpgradeRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UpgradeRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<List<UpgradeRule>> GetRulesAsync(CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        IEnumerable<UpgradeRule> result = await connection.QueryAsyncWithRetry<UpgradeRule>(new CommandDefinition("""
                                                                                                                  select id, block, upgradechoice
                                                                                                                  from upgraderule 
                                                                                                                  """, cancellationToken: token));

        return result.ToList();
    }

    public async Task<bool> UpsertUpgrades(Guid characterId, List<Upgrade> upgrades, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update character
                                                                                  set upgrades = @Upgrades
                                                                                  where id = @characterId
                                                                                  """, new
                                                                                       {
                                                                                           characterId,
                                                                                           Upgrades = new JsonParameter(JsonSerializer.Serialize(upgrades))
                                                                                       }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }
}