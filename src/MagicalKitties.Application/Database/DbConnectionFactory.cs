using System.Data;
using Dapper;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.MagicalPowers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using Serilog;
using Attribute = MagicalKitties.Application.Models.Characters.Attribute;
using ILogger = Serilog.ILogger;

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
        SqlMapper.AddTypeHandler(typeof(List<MagicalPower>), new JsonTypeHandler());
    }

    // public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    // {
    //     NpgsqlConnection connection = new(_connectionString);
    //     await connection.OpenAsync(token);
    //     return connection;
    // }

    public string GetConnectionString()
    {
        return _connectionString;
    }
}