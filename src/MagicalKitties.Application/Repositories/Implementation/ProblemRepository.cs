using System.Data;
using Dapper;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.Humans.Updates;
using MagicalKitties.Application.Models.Rules;
using MagicalKitties.Application.Services;

namespace MagicalKitties.Application.Repositories.Implementation;

public class ProblemRepository : IProblemRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ProblemRepository(IDbConnectionFactory dbConnectionFactory, IDateTimeProvider dateTimeProvider)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> CreateProblemAsync(Problem problem, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  insert into problem(id, human_id, source, emotion, rank, solved, deleted_utc)
                                                                                  values (@Id, @HumanId, @Source, @Emotion, @Rank, @Solved, @DeletedUtc)
                                                                                  """, new
                                                                                       {
                                                                                           problem.Id,
                                                                                           problem.HumanId,
                                                                                           problem.Source,
                                                                                           problem.Emotion,
                                                                                           problem.Rank,
                                                                                           problem.Solved,
                                                                                           problem.DeletedUtc
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QuerySingleAsyncWithRetry<bool>(new CommandDefinition("""
                                                                                      select exists(select 1
                                                                                      from problem
                                                                                      where id = @id)
                                                                                      """, new { id }, cancellationToken: token));
    }

    public async Task<bool> UpdateSourceAsync(ProblemUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update problem
                                                                                  set source = @Source
                                                                                  where id = @ProblemId
                                                                                  """, new
                                                                                       {
                                                                                           update.Source,
                                                                                           update.ProblemId
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateEmotionAsync(ProblemUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update problem
                                                                                  set emotion = @Emotion
                                                                                  where id = @ProblemId
                                                                                  """, new
                                                                                       {
                                                                                           update.Emotion,
                                                                                           update.ProblemId
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateRankAsync(ProblemUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update problem
                                                                                  set rank = @Rank
                                                                                  where id = @ProblemId
                                                                                  """, new
                                                                                       {
                                                                                           update.Rank,
                                                                                           update.ProblemId
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateSolvedAsync(ProblemUpdate update, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update problem
                                                                                  set solved = @Solved
                                                                                  where id = @ProblemId
                                                                                  """, new
                                                                                       {
                                                                                           update.Solved,
                                                                                           update.ProblemId
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid problemId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsyncWithRetry(new CommandDefinition("""
                                                                                  update problem
                                                                                  set deleted_utc = @Now
                                                                                  where id = @ProblemId
                                                                                  """, new
                                                                                       {
                                                                                           Now = _dateTimeProvider.GetUtcNow(),
                                                                                           problemId
                                                                                       }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<List<ProblemRule>> GetAllProblemSourcesAsync(CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        IEnumerable<ProblemRule> result = await connection.QueryAsyncWithRetry<ProblemRule>(new CommandDefinition("""
                                                                                                                              select id, id, roll_value, source, custom_source from problemsource
                                                                                                                              """, cancellationToken: token));

        return result.ToList();
    }

    public async Task<List<ProblemRule>> GetAllEmotionsAsync(CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        IEnumerable<ProblemRule> result = await connection.QueryAsyncWithRetry<ProblemRule>(new CommandDefinition("""
                                                                                                                  select id, id, roll_value, source, custom_source from emotionsource
                                                                                                                  """, cancellationToken: token));

        return result.ToList();
    }
}