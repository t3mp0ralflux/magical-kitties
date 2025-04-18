using FluentValidation;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Repositories.Implementation;
using MagicalKitties.Application.Services;
using MagicalKitties.Application.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace MagicalKitties.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        #region Repositories

        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddSingleton<IGlobalSettingsRepository, GlobalSettingsRepository>();
        services.AddSingleton<IEmailRepository, EmailRepository>();
        services.AddSingleton<ICharacterRepository, CharacterRepository>();
        services.AddSingleton<IFlawRepository, FlawRepository>();
        services.AddSingleton<ITalentRepository, TalentRepository>();

        #endregion

        #region Services

        services.AddSingleton<IAccountService, AccountService>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IGlobalSettingsService, GlobalSettingsService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<ICharacterService, CharacterService>();
        services.AddSingleton<IFlawService, FlawService>();
        services.AddSingleton<ITalentService, TalentService>();

        #endregion

        #region Validators

        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton); // set to singleton as it'll be one.

        #endregion

        #region Other

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        #endregion

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
        services.AddSingleton<IEmailService, EmailService>();

        services.AddHostedService<HostedServices.EmailService>();

        return services;
    }
}