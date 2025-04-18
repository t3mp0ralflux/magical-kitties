using System.Data;
using Dapper;
using MagicalKitties.Application.Models.Characters;
using Npgsql;
using Attribute = MagicalKitties.Application.Models.Characters.Attribute;

namespace MagicalKitties.Application.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;

        SqlMapper.AddTypeHandler(typeof(List<Attribute>), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(List<Endowment>), new JsonTypeHandler());
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync(token);
        return connection;
    }
}