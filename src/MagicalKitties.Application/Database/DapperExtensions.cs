﻿using System.Data;
using System.Net.Sockets;
using Dapper;
using Npgsql;
using Polly;
using Polly.Retry;
using Serilog;

namespace MagicalKitties.Application.Database;

// https://hyr.mn/dapper-and-polly
public static class DapperExtensions
{
    private static readonly IEnumerable<TimeSpan> RetryTimes =
    [
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(3)
    ];

    private static readonly AsyncRetryPolicy RetryPolicy = Policy
                                                           .HandleInner<TimeoutException>()
                                                           .OrInner<SocketException>()
                                                           .WaitAndRetryAsync(RetryTimes, (exception, timeSpan, retryCount, _) =>
                                                                                          {
                                                                                              Log.Warning(exception, "WARNING: Error talking to DB at {ConnectionString}, will retry after {RetryTimeSpan}. Retry attempt {RetryCount}",
                                                                                                  "test",
                                                                                                  timeSpan,
                                                                                                  retryCount);
                                                                                          });


    public static async Task<IDbConnection> CreateConnectionAsync(this IDbConnectionFactory connectionFactory, CancellationToken token = default)
    {
        NpgsqlConnection connection = new(connectionFactory.GetConnectionString());
        await RetryPolicy.ExecuteAsync(async ctx => await connection.OpenAsync(ctx), token);

        return connection;
    }

    public static async Task<int> ExecuteAsyncWithRetry(this IDbConnection cnn, CommandDefinition commandDefinition)
    {
        return await RetryPolicy.ExecuteAsync(async () => await cnn.ExecuteAsync(commandDefinition));
    }

    public static async Task<IEnumerable<T>> QueryAsyncWithRetry<T>(this IDbConnection cnn, CommandDefinition commandDefinition)
    {
        return await RetryPolicy.ExecuteAsync(async () => await cnn.QueryAsync<T>(commandDefinition));
    }

    public static async Task<IEnumerable<T>> QueryAsyncWithRetry<T, TFirst>(this IDbConnection cnn, CommandDefinition commandDefinition, Func<T, TFirst, T> map, string splitOn)
    {
        return await RetryPolicy.ExecuteAsync(async () => await cnn.QueryAsync(commandDefinition, map, splitOn));
    }
    
    public static async Task<IEnumerable<T>> QueryAsyncWithRetry<T, TFirst, TSecond, TThird, TFourth>(this IDbConnection cnn, CommandDefinition commandDefinition, Func<T, TFirst, TSecond, TThird, TFourth, T> map, string splitOn)
    {
        return await RetryPolicy.ExecuteAsync(async () => await cnn.QueryAsync(commandDefinition, map, splitOn));
    }
    
    public static async Task<T> QuerySingleAsyncWithRetry<T>(this IDbConnection cnn, CommandDefinition commandDefinition)
    {
        return await RetryPolicy.ExecuteAsync(async () => await cnn.QuerySingleAsync<T>(commandDefinition));
    }

    public static async Task<T?> QuerySingleOrDefaultAsyncWithRetry<T>(this IDbConnection cnn, CommandDefinition commandDefinition)
    {
        return await RetryPolicy.ExecuteAsync(async () => await cnn.QuerySingleOrDefaultAsync<T?>(commandDefinition));
    }
}