using Bogus;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.GlobalSettings;
using MagicalKitties.Application.Models.System;
using MagicalKitties.Application.Validators.Characters;

namespace Testing.Common;

public static class Fakes
{
    public static Account GenerateAccount(AccountStatus? status = AccountStatus.active, AccountRole? role = AccountRole.admin, string? userName = null, bool isDeleted = false, bool isReset = false)
    {
        Faker<Account> fakeAccount = new Faker<Account>()
                                     .RuleFor(x => x.Id, f => Guid.NewGuid())
                                     .RuleFor(x => x.FirstName, f => f.Person.FirstName)
                                     .RuleFor(x => x.LastName, f => f.Person.LastName)
                                     .RuleFor(x => x.Username, f => string.IsNullOrWhiteSpace(userName) ? f.Internet.UserName() : userName)
                                     .RuleFor(x => x.Email, f => f.Person.Email)
                                     .RuleFor(x => x.Password, f => f.Internet.Password())
                                     .RuleFor(x => x.AccountStatus, f => status)
                                     .RuleFor(x => x.AccountRole, f => role)
                                     .RuleFor(x => x.CreatedUtc, f => f.Date.Recent())
                                     .RuleFor(x => x.UpdatedUtc, f => f.Date.Recent())
                                     .RuleFor(x => x.LastLoginUtc, f => f.Date.Recent())
                                     .RuleFor(x => x.DeletedUtc, _ => isDeleted ? DateTime.UtcNow : null)
                                     .RuleFor(x => x.PasswordResetRequestedUtc, _ => isReset ? DateTime.UtcNow : null)
                                     .RuleFor(x => x.PasswordResetCode, _ => isReset ? "ResetCode" : null);

        return fakeAccount;
    }

    public static EmailData GenerateEmailData(DateTime? sendAfterUtc = null)
    {
        Faker<EmailData> fakeSetting = new Faker<EmailData>()
                                       .RuleFor(x => x.Id, _ => Guid.NewGuid())
                                       .RuleFor(x => x.ShouldSend, _ => true)
                                       .RuleFor(x => x.SendAttempts, f => 0)
                                       .RuleFor(x => x.SendAfterUtc, f => sendAfterUtc ??= f.Date.Recent())
                                       .RuleFor(x => x.SenderEmail, f => f.Person.Email)
                                       .RuleFor(x => x.RecipientEmail, f => f.Person.Email)
                                       .RuleFor(x => x.SenderAccountId, _ => Guid.NewGuid())
                                       .RuleFor(x => x.ReceiverAccountId, _ => Guid.NewGuid())
                                       .RuleFor(x => x.ResponseLog, f => f.System.FileType())
                                       .RuleFor(x => x.Body, f => f.Internet.ExampleEmail());

        return fakeSetting;
    }

    public static GlobalSetting GenerateGlobalSetting(string? value = null)
    {
        Faker<GlobalSetting>? fakeSetting = new Faker<GlobalSetting>()
                                            .RuleFor(x => x.Id, _ => Guid.NewGuid())
                                            .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                                            .RuleFor(x => x.Value, f => value ??= f.Hacker.Noun());

        return fakeSetting;
    }

    public static Character GenerateNewCharacter(Account account)
    {
        Faker<Character> fakeCharacter = new Faker<Character>()
                                         .RuleFor(x => x.Id, _ => Guid.NewGuid())
                                         .RuleFor(x => x.AccountId, _ => account.Id)
                                         .RuleFor(x => x.Username, _ => account.Username)
                                         .RuleFor(x => x.Name, f => f.Person.FullName)
                                         .RuleFor(x => x.MaxOwies, _ => 2)
                                         .RuleFor(x => x.StartingTreats, _ => 2)
                                         .RuleFor(x => x.Cunning, _ => 0)
                                         .RuleFor(x => x.Cute, _ => 0)
                                         .RuleFor(x => x.Fierce, _ => 0);

        return fakeCharacter;
    }

    public static Character GenerateCharacter(Account account)
    {
        Faker<Character> fakeCharacter = new Faker<Character>()
                                         .RuleFor(x => x.Id, _ => Guid.NewGuid())
                                         .RuleFor(x => x.AccountId, _ => account.Id)
                                         .RuleFor(x => x.Username, _ => account.Username)
                                         .RuleFor(x => x.Name, f => f.Person.FullName)
                                         .RuleFor(x => x.CreatedUtc, _ => DateTime.UtcNow)
                                         .RuleFor(x => x.UpdatedUtc, _ => DateTime.UtcNow)
                                         .RuleFor(x => x.MaxOwies, _ => 2)
                                         .RuleFor(x => x.StartingTreats, _ => 2)
                                         .RuleFor(x => x.Cunning, _ => 0)
                                         .RuleFor(x => x.Cute, _ => 0)
                                         .RuleFor(x => x.Fierce, _ => 0);

        return fakeCharacter;
    }

    public static AttributeUpdateValidationContext GenerateValidationContext(Guid? accountId = null, Guid? characterId = null, int? cunning = null, int? cute = null, int? fierce = null, AttributeOption? attributeOption = null, EndowmentChange? magicalPowerChange = null, EndowmentChange? flawChange = null, EndowmentChange? talentChange = null, int? currentTreats = null, int? level = null, int? owies = null)
    {
        Account account = GenerateAccount();
        Character character = GenerateCharacter(account);

        AttributeUpdate update = new()
                                 {
                                     AccountId = accountId ?? account.Id,
                                     CharacterId = characterId ?? character.Id,
                                     Cunning = cunning,
                                     Cute = cute,
                                     Fierce = fierce,
                                     AttributeOption = attributeOption ?? AttributeOption.cunning,
                                     CurrentTreats = currentTreats,
                                     FlawChange = flawChange,
                                     Level = level,
                                     MagicalPowerChange = magicalPowerChange,
                                     TalentChange = talentChange,
                                     Owies = owies
                                 };

        AttributeUpdateValidationContext fakeContext = new()
                                                       {
                                                           Character = character,
                                                           Update = update
                                                       };

        return fakeContext;
    }
}