using System.Data;
using Dapper;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.MagicalPowers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Attribute = MagicalKitties.Application.Models.Characters.Attribute;

namespace MagicalKitties.Application.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    private readonly IAsyncPolicy _retryPolicy;
    
    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;

        SqlMapper.AddTypeHandler(typeof(List<Attribute>), new JsonTypeHandler());
        SqlMapper.AddTypeHandler(typeof(List<MagicalPower>), new JsonTypeHandler());
        
        var retryPolicy = Policy
                          .Handle<NpgsqlException>()
                          .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(1.5), onRetry: (exception, timeSpan, retryCount, context) =>
                                                                                                    {
                                                                                                        Console.WriteLine($"Connection lost, retry attempt {retryCount} at {DateTime.Now} . Exception Message: {exception.Message}" + Environment.NewLine);
                                                                                                    });

        var fallbackPolicy = Policy.Handle<NpgsqlException>().FallbackAsync(fallbackAction: cancellationToken => Task.CompletedTask, onFallbackAsync: async e =>
                                                                                                                                                      {
                                                                                                                                                          await Task.Run(() => Console.WriteLine($"Failed after maximum retries. Exception Message: {e.Message}" + Environment.NewLine));
                                                                                                                                                      });
        _retryPolicy = Policy.WrapAsync(fallbackPolicy, retryPolicy);
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        NpgsqlConnection connection = new(_connectionString);
        
        int attempt = 0;

        await _retryPolicy.ExecuteAsync(async (ctx) =>
                                        {
                                            attempt++;
                                            await connection.OpenAsync(ctx);
                                        }, cancellationToken: token);
        
        return connection;
    }
}