using FluentValidation;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.HostedServices;
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
        services.AddSingleton<ICharacterUpdateRepository, CharacterUpdateRepository>();
        services.AddSingleton<IFlawRepository, FlawRepository>();
        services.AddSingleton<ITalentRepository, TalentRepository>();
        services.AddSingleton<IMagicalPowerRepository, MagicalPowerRepository>();
        services.AddSingleton<IHumanRepository, HumanRepository>();
        services.AddSingleton<IProblemRepository, ProblemRepository>();
        services.AddSingleton<IUpgradeRepository, UpgradeRepository>();

        #endregion

        #region Services

        services.AddSingleton<IAccountService, AccountService>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IGlobalSettingsService, GlobalSettingsService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<ICharacterService, CharacterService>();
        services.AddSingleton<ICharacterUpdateService, CharacterUpdateService>();
        services.AddSingleton<IFlawService, FlawService>();
        services.AddSingleton<ITalentService, TalentService>();
        services.AddSingleton<IMagicalPowerService, MagicalPowerService>();
        services.AddSingleton<IHumanService, HumanService>();
        services.AddSingleton<ICharacterUpgradeService, CharacterUpgradeService>();
        services.AddSingleton<IRuleService, RuleService>();

        #endregion

        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Transient); // set to singleton as it'll be one.

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
        services.AddSingleton<IEmailService, EmailService>();

        services.AddHostedService<EmailProcessingService>();

        return services;
    }

    public static IServiceCollection AddResilience(this IServiceCollection services)
    {
        return services;
    }
}