using Bogus;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.GlobalSettings;
using MagicalKitties.Application.Models.System;
using Attribute = MagicalKitties.Application.Models.Characters.Attribute;

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
        Faker<EmailData>? fakeSetting = new Faker<EmailData>()
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
        Faker<Attribute> attributesFaker = new Faker<Attribute>()
                                           .RuleFor(x => x.Id, _ => Guid.NewGuid())
                                           .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                                           .RuleFor(x => x.Value, f => f.Random.Int(1, 3));

        Faker<Character>? fakeCharacter = new Faker<Character>()
                                          .RuleFor(x => x.Id, _ => Guid.NewGuid())
                                          .RuleFor(x => x.AccountId, _ => account.Id)
                                          .RuleFor(x => x.Username, _ => account.Username)
                                          .RuleFor(x => x.Name, f => f.Person.FullName)
                                          .RuleFor(x => x.Attributes, _ => attributesFaker.Generate(3))
                                          .RuleFor(x => x.MaxOwies, _ => 2)
                                          .RuleFor(x => x.StartingTreats, _ => 2);

        return fakeCharacter;
    }

    public static Character GenerateCharacter(Account account)
    {
        Faker<Attribute> attributesFaker = new Faker<Attribute>()
                                           .RuleFor(x => x.Id, _ => Guid.NewGuid())
                                           .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                                           .RuleFor(x => x.Value, f => f.Random.Int(1, 3));

        Faker<Character>? fakeCharacter = new Faker<Character>()
                                          .RuleFor(x => x.Id, _ => Guid.NewGuid())
                                          .RuleFor(x => x.AccountId, _ => account.Id)
                                          .RuleFor(x => x.Username, _ => account.Username)
                                          .RuleFor(x => x.Name, f => f.Person.FullName)
                                          .RuleFor(x => x.CreatedUtc, _ => DateTime.UtcNow)
                                          .RuleFor(x => x.UpdatedUtc, _ => DateTime.UtcNow)
                                          .RuleFor(x => x.Attributes, _ => attributesFaker.Generate(3))
                                          .RuleFor(x => x.MaxOwies, _ => 2)
                                          .RuleFor(x => x.StartingTreats, _ => 2);

        return fakeCharacter;
    }
}