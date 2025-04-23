using System.Data;
using Dapper;
using MagicalKitties.Application.Database;
using Microsoft.AspNetCore.Mvc;

namespace MagicalKitties.Api.Controllers;

[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DebugController(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    [HttpGet("timeout")]
    public async Task<IActionResult> CheckTimeout()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var result = await connection.QueryAsyncWithRetry<bool>(new CommandDefinition(""" select pg_sleep(200); """, commandTimeout: 3));

        return Ok(result);
    }
}