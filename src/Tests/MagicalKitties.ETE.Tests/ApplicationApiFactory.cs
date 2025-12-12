using MagicalKitties.Api;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.HostedServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Testcontainers.PostgreSql;
using Xunit;

namespace MagicalKitties.ETE.Tests;

public class ApplicationApiFactory: WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
                                                        .WithDatabase("testdb")
                                                        .WithUsername("integration")
                                                        .WithPassword("tests")
                                                        .Build();
    
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        foreach (string file in Directory.GetFiles("../../../../../scripts").Order())
        {
            string script = await File.ReadAllTextAsync(file);
            await _dbContainer.ExecScriptAsync(script);
        }
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
                                  {
                                      ServiceDescriptor? descriptor = services.SingleOrDefault(x => x.ImplementationType == typeof(EmailProcessingService));

                                      if (descriptor is not null)
                                      {
                                          services.Remove(descriptor);
                                      }
                                  });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
                                 {
                                     logging.ClearProviders();
                                 });

        builder.ConfigureTestServices(services =>
                                      {
                                          services.RemoveAll<IDbConnectionFactory>();
                                          services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(_dbContainer.GetConnectionString()));
                                      });

        builder.UseEnvironment("Testing");
        
    }
}