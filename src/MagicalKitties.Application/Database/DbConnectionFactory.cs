using Dapper;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Rules;
using MagicalKitties.Application.Models.Talents;

namespace MagicalKitties.Application.Database;

public interface IDbConnectionFactory
{
    string GetConnectionString();
}

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
        
        SqlMapper.AddTypeHandler(typeof(Problem), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(List<Talent>), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(List<MagicalPower>), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(List<Human>), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(List<Problem>), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(List<Upgrade>), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(object), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(Flaw), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(ProblemRule), new JsonTypeHandler());

        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public string GetConnectionString()
    {
        return _connectionString;
    }
}