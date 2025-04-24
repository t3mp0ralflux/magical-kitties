using Dapper;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Talents;
using Attribute = MagicalKitties.Application.Models.Characters.Attribute;

namespace MagicalKitties.Application.Database;

public interface IDbConnectionFactory
{
    //Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
    string GetConnectionString();
}

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;

        SqlMapper.AddTypeHandler(typeof(List<Attribute>), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(Flaw), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(List<Talent>), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(List<MagicalPower>), new JsonTypeHandler());

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
    
    public string GetConnectionString()
    {
        return _connectionString;
    }
}