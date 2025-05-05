using Bogus;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.GlobalSettings;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.System;
using MagicalKitties.Application.Models.Talents;
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

    public static Character WithBaselineData(this Character character)
    {
        character.Cunning = 3;
        character.Cute = 2;
        character.Fierce = 1;
        character.Level = 5;
        character.Flaw = new Flaw
                         {
                             Id = 11,
                             Name = "Amnesia",
                             Description = "You are missing a part of your past, having lost some or all of your memory.",
                             IsCustom = false
                         };
        character.Talents =
        [
            new Talent
            {
                Id = 22,
                Name = "Claws",
                Description = "You are very proud of your razor-sharp claws, and can use them in all sorts of clever ways.",
                IsCustom = false
            }
        ];
        character.MagicalPowers =
        [
            new MagicalPower
            {
                Id = 33,
                Name = "Invisibility",
                Description = "You can turn invisible. Nobody can see you, but they can still hear, smell, and touch you. Objects you wear or carry are still visible.",
                IsCustom = false,
                BonusFeatures =
                [
                    new MagicalPower
                    {
                        Id = 1,
                        Name = "Share Invisibility",
                        Description = "You can also make friends near you invisible.",
                        IsCustom = false
                    },
                    new MagicalPower
                    {
                        Id = 2,
                        Name = "Object Invisibility",
                        Description = "You can turn any object you touch invisible.",
                        IsCustom = false
                    },
                    new MagicalPower
                    {
                        Id = 3,
                        Name = "See Invisibility",
                        Description = "You can see other creatures who are invisible.",
                        IsCustom = false
                    },
                    new MagicalPower
                    {
                        Id = 4,
                        Name = "Soundless",
                        Description = "In addition to being invisible, you also make no sound.",
                        IsCustom = false
                    },
                    new MagicalPower
                    {
                        Id = 5,
                        Name = "Scentless",
                        Description = "In addition to being invisible, you also can't be detected by smell.",
                        IsCustom = false
                    }
                ]
            }
        ];
        character.CurrentOwies = 2;
        character.CurrentTreats = 6;
        character.CurrentInjuries = 1;

        return character;
    }

    public static Character WithHumanData(this Character character)
    {
        List<Human> humanCollection = Enumerable.Range(1,3).Select(_=> GenerateHuman(character.Id)).ToList();
        
        foreach (Human human in humanCollection)
        {
            Faker<Problem> problemFaker = new Faker<Problem>()
                                           .RuleFor(x=>x.Id, _ => Guid.NewGuid())
                                           .RuleFor(x=>x.HumanId, _ => human.Id)
                                           .RuleFor(x=>x.Rank, f => f.Random.Int(1,5))
                                           .RuleFor(x=>x.Source, f => f.Lorem.Word())
                                           .RuleFor(x=>x.Emotion, f=>f.Lorem.Word());

            human.Problems = problemFaker.Generate(3);
        }
        
        character.Humans = humanCollection;
            
        return character;
    }

    public static Human GenerateHuman(Guid characterId)
    {
        var fakeHuman = new Faker<Human>()
                        .RuleFor(x => x.Id, _ => Guid.NewGuid())
                        .RuleFor(x => x.CharacterId, _ => characterId)
                        .RuleFor(x => x.Name, f => f.Person.FullName)
                        .RuleFor(x=>x.Description, f=>f.Lorem.Sentences(3));

        return fakeHuman;
    }

    public static AttributeUpdate GenerateAttributeUpdate(Guid accountId, Guid characterId, AttributeOption option)
    {
        return new AttributeUpdate
               {
                   AccountId = accountId,
                   AttributeOption = option,
                   CharacterId = characterId,
                   Cunning = 3,
                   Cute = 2,
                   Fierce = 1,
                   Level = 5,
                   FlawChange = new EndowmentChange
                                {
                                    NewId = 11,
                                    PreviousId = 11
                                },
                   TalentChange = new EndowmentChange
                                  {
                                      NewId = 22,
                                      PreviousId = 22
                                  },
                   MagicalPowerChange = new EndowmentChange
                                        {
                                            NewId = 33,
                                            PreviousId = 33
                                        },
                   CurrentOwies = 2,
                   CurrentTreats = 6,
                   CurrentInjuries = 1
               };
    }

    public static AttributeUpdateValidationContext GenerateValidationContext(Guid? accountId = null, Guid? characterId = null, int? cunning = null, int? cute = null, int? fierce = null, AttributeOption? attributeOption = null, EndowmentChange? magicalPowerChange = null, EndowmentChange? flawChange = null, EndowmentChange? talentChange = null, int? currentTreats = null, int? level = null, int? currentOwies = null, int? currentInjuries = null)
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
                                     CurrentOwies = currentOwies,
                                     CurrentInjuries = currentInjuries
                                 };

        AttributeUpdateValidationContext fakeContext = new()
                                                       {
                                                           Character = character,
                                                           Update = update
                                                       };

        return fakeContext;
    }
}