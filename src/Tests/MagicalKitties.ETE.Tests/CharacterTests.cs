using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Dapper;
using FluentAssertions;
using FluentValidation;
using MagicalKitties.Api.Controllers;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Repositories.Implementation;
using MagicalKitties.Application.Services;
using MagicalKitties.Application.Services.Implementation;
using MagicalKitties.Application.Validators.Characters;
using MagicalKitties.Contracts.Responses.Auth;
using MagicalKitties.Contracts.Responses.Characters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Testing.Common;
using Xunit;

namespace MagicalKitties.ETE.Tests;

public class CharacterTests: IClassFixture<ApplicationApiFactory>, IDisposable
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    private readonly IAuthenticationHandler _authHandler = Substitute.For<IAuthenticationHandler>();
    private readonly HttpClient _httpClient;

    public CharacterTests(ApplicationApiFactory apiFactory)
    {
        IDbConnectionFactory dbConnectionFactory = apiFactory.Services.GetRequiredService<IDbConnectionFactory>();
        _accountRepository = new AccountRepository(dbConnectionFactory, _dateTimeProvider);
        ICharacterRepository characterRepository = new CharacterRepository(dbConnectionFactory, _dateTimeProvider);
        IMagicalPowerRepository magicalPowerRepository = new MagicalPowerRepository(dbConnectionFactory);
        ITalentRepository talentRepository = new TalentRepository(dbConnectionFactory);
        DateTime now = DateTime.UtcNow;

        _dateTimeProvider.GetUtcNow().Returns(now);

        _httpClient = apiFactory.WithWebHostBuilder(builder =>
                                                    {
                                                        builder.ConfigureTestServices(services =>
                                                                                      {
                                                                                          services
                                                                                              .AddAuthentication(defaultScheme: "TestScheme")
                                                                                              .AddScheme<AuthenticationSchemeOptions, TestingAuthHandler>("TestScheme", _ => {});
                                                                                          
                                                                                          services.RemoveAll<IDateTimeProvider>();
                                                                                          services.RemoveAll<ICharacterRepository>();
                                                                                          services.RemoveAll<IMagicalPowerRepository>();
                                                                                          services.RemoveAll<ITalentRepository>();
                                                                                          services.RemoveAll<IAccountRepository>();

                                                                                          services.AddTransient<IDateTimeProvider>(_ => _dateTimeProvider);
                                                                                          services.AddTransient<ICharacterRepository>(_ => characterRepository);
                                                                                          services.AddTransient<IAccountRepository>(_ => _accountRepository);
                                                                                          services.AddTransient<IMagicalPowerRepository>(_ => magicalPowerRepository);
                                                                                          services.AddTransient<ITalentRepository>(_ => talentRepository);
                                                                                      });
                                                    }).CreateClient();
    }
    
    public void Dispose()
    {
    }

    [SkipIfEnvironmentMissingFact]
    public async Task Character_ShouldBeCreated_WhenInformationIsProvided()
    {
        // Arrange
        const string requestPath = "/api/characters";
        
        Account account = Fakes.GenerateAccount(email: "test@test.com");
        account.Password = "password";

        await _accountRepository.CreateAsync(account);
        
        // Act
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
        
        HttpResponseMessage result = await _httpClient.PostAsync(requestPath, null, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        CharacterResponse? response = await result.Content.ReadFromJsonAsync<CharacterResponse>();
        response.Should().NotBeNull();
        response.AccountId.Should().Be(account.Id);
        response.Name.Should().Contain(account.Username);
    }
}