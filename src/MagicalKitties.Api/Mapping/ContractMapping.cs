using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Auth;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.GlobalSettings;
using MagicalKitties.Application.Models.Humans.Updates;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Rules;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Contracts.Requests.Account;
using MagicalKitties.Contracts.Requests.Auth;
using MagicalKitties.Contracts.Requests.Endowments.Flaws;
using MagicalKitties.Contracts.Requests.Endowments.MagicalPowers;
using MagicalKitties.Contracts.Requests.Endowments.Talents;
using MagicalKitties.Contracts.Requests.GlobalSetting;
using MagicalKitties.Contracts.Responses.Account;
using MagicalKitties.Contracts.Responses.Auth;
using MagicalKitties.Contracts.Responses.Characters;
using MagicalKitties.Contracts.Responses.Flaws;
using MagicalKitties.Contracts.Responses.GlobalSetting;
using MagicalKitties.Contracts.Responses.Humans;
using MagicalKitties.Contracts.Responses.MagicalPowers;
using MagicalKitties.Contracts.Responses.Rules;
using MagicalKitties.Contracts.Responses.Talents;
using DescriptionOption = MagicalKitties.Application.Models.Humans.Updates.DescriptionOption;
using DescriptionUpdate = MagicalKitties.Application.Models.Characters.Updates.DescriptionUpdate;
using MKCtr = MagicalKitties.Contracts.Models;
using MKCtrCharacterRequests = MagicalKitties.Contracts.Requests.Characters;
using MKAppCharacters = MagicalKitties.Application.Models.Characters;
using MKCtrHumanRequests = MagicalKitties.Contracts.Requests.Humans;
using MKAppHumans = MagicalKitties.Application.Models.Humans;

namespace MagicalKitties.Api.Mapping;

public static class ContractMapping
{
    #region Accounts

    public static Account ToAccount(this AccountCreateRequest request)
    {
        return new Account
               {
                   Id = Guid.NewGuid(),
                   FirstName = request.FirstName,
                   LastName = request.LastName,
                   Username = request.UserName,
                   Password = request.Password,
                   Email = request.Email.ToLowerInvariant()
               };
    }

    public static Account ToAccount(this AccountUpdateRequest request, Guid id)
    {
        return new Account
               {
                   Id = id,
                   FirstName = request.FirstName,
                   LastName = request.LastName,
                   Username = string.Empty, // not used, but required
                   Email = string.Empty, // not used, but required
                   Password = string.Empty, // not used, but required
                   AccountStatus = (AccountStatus)request.AccountStatus,
                   AccountRole = (AccountRole)request.AccountRole
               };
    }

    public static AccountResponse ToResponse(this Account account)
    {
        return new AccountResponse
               {
                   Id = account.Id,
                   FirstName = account.FirstName,
                   LastName = account.LastName,
                   Email = account.Email,
                   UserName = account.Username,
                   AccountRole = (MKCtr.AccountRole)account.AccountRole,
                   AccountStatus = (MKCtr.AccountStatus)account.AccountStatus,
                   LastLogin = account.LastLoginUtc
               };
    }

    public static AccountsResponse ToResponse(this IEnumerable<Account> accounts, int page, int pageSize, int totalCount)
    {
        return new AccountsResponse
               {
                   Items = accounts.Select(ToResponse),
                   Page = page,
                   PageSize = pageSize,
                   Total = totalCount
               };
    }

    public static GetAllAccountsOptions ToOptions(this GetAllAccountsRequest request)
    {
        string? sortField = request.SortBy?.Trim('+', '-');
        if (sortField is not null && sortField == "lastlogin")
        {
            sortField = "last_login_utc";
        }

        return new GetAllAccountsOptions
               {
                   UserName = request.UserName,
                   AccountStatus = (AccountStatus?)request.AccountStatus,
                   AccountRole = (AccountRole?)request.AccountRole,
                   SortField = sortField,
                   SortOrder = request.SortBy is null ? SortOrder.unordered : request.SortBy.StartsWith('-') ? SortOrder.descending : SortOrder.ascending,
                   Page = request.Page,
                   PageSize = request.PageSize
               };
    }

    public static AccountActivationResponse ToResponse(this AccountActivation activation)
    {
        return new AccountActivationResponse
               {
                   Username = activation.Username
               };
    }

    #endregion

    #region Auth

    public static PasswordReset ToReset(this PasswordResetRequest request)
    {
        return new PasswordReset
               {
                   Email = request.Email,
                   Password = request.Password,
                   ResetCode = request.ResetCode
               };
    }

    public static PasswordResetResponse ToResponse(this PasswordReset passwordReset)
    {
        return new PasswordResetResponse
               {
                   Email = passwordReset.Email
               };
    }

    public static AccountLogin ToLogin(this Account account)
    {
        return new AccountLogin
               {
                   Email = account.Email
               };
    }

    #endregion

    #region Characters

    public static CharactersResponse ToGetAllResponse(this IEnumerable<MKAppCharacters.Character> characters, int page, int pageSize, int total)
    {
        return new CharactersResponse
               {
                   Items = characters.Select(x => new GetAllCharacterResponse
                                                  {
                                                      AccountId = x.AccountId,
                                                      Id = x.Id,
                                                      Name = x.Name,
                                                      Username = x.Username
                                                  }),
                   Page = page,
                   PageSize = pageSize,
                   Total = total
               };
    }

    public static CharacterResponse ToResponse(this MKAppCharacters.Character character)
    {
        return new CharacterResponse
               {
                   Id = character.Id,
                   AccountId = character.AccountId,
                   Name = character.Name,
                   Flaw = character.Flaw?.ToResponse(),
                   Talents = character.Talents.Select(ToResponse).ToList(),
                   MagicalPowers = character.MagicalPowers.Select(ToResponse).ToList(),
                   CurrentInjuries = character.CurrentInjuries,
                   CurrentOwies = character.CurrentOwies,
                   MaxOwies = character.MaxOwies,
                   CurrentTreats = character.CurrentTreats,
                   StartingTreats = character.StartingTreats,
                   CurrentXp = character.CurrentXp,
                   Description = character.Description,
                   Hometown = character.Hometown,
                   Human = character.Humans.Select(ToResponse).ToList(),
                   Level = character.Level,
                   Cunning = character.Cunning,
                   Cute = character.Cute,
                   Fierce = character.Fierce,
                   Upgrades = character.Upgrades
               };
    }

    public static MKAppCharacters.GetAllCharactersOptions ToOptions(this MKCtrCharacterRequests.GetAllCharactersRequest request, Guid accountId)
    {
        return new MKAppCharacters.GetAllCharactersOptions
               {
                   AccountId = accountId,
                   Name = request.Name,
                   Class = request.Class,
                   Level = request.Level,
                   Species = request.Species,
                   Page = request.Page,
                   PageSize = request.PageSize,
                   SortField = request.SortBy,
                   SortOrder = request.SortBy is null ? SortOrder.unordered : request.SortBy.StartsWith('-') ? SortOrder.descending : SortOrder.ascending
               };
    }

    #endregion

    #region CharacterUpdates

    public static DescriptionUpdate ToUpdate(this MKCtrCharacterRequests.CharacterDescriptionUpdateRequest request, Guid accountId)
    {
        return new DescriptionUpdate
               {
                   AccountId = accountId,
                   CharacterId = request.CharacterId,
                   Name = request.Name,
                   Description = request.Description,
                   Hometown = request.Hometown,
                   XP = request.XP
               };
    }

    public static AttributeUpdate ToUpdate(this MKCtrCharacterRequests.CharacterAttributeUpdateRequest request, Guid accountId)
    {
        return new AttributeUpdate
               {
                   AccountId = accountId,
                   CharacterId = request.CharacterId,
                   Cunning = request.Cunning,
                   Cute = request.Cute,
                   Fierce = request.Fierce,
                   Level = request.Level,
                   FlawChange = request.FlawChange?.ToUpdate(),
                   TalentChange = request.TalentChange?.ToUpdate(),
                   MagicalPowerChange = request.MagicalPowerChange?.ToUpdate(),
                   CurrentOwies = request.CurrentOwies,
                   CurrentTreats = request.CurrentTreats,
                   CurrentInjuries = request.CurrentInjuries,
                   Incapacitated = request.Incapacitated
               };
    }

    public static EndowmentChange ToUpdate(this MKCtrCharacterRequests.EndowmentChangeRequest request)
    {
        return new EndowmentChange
               {
                   PreviousId = request.PreviousId,
                   NewId = request.NewId
               };
    }

    public static MKAppCharacters.UpgradeRequest ToUpdate(this MKCtrCharacterRequests.UpgradeUpsertRequest request, Guid accountId, Guid characterId)
    {
        return new MKAppCharacters.UpgradeRequest
               {
                   AccountId = accountId,
                   CharacterId = characterId,
                   UpgradeOption = (UpgradeOption)request.UpgradeOption,
                   Upgrade = new MKAppCharacters.Upgrade
                             {
                                 Id = request.UpgradeId,
                                 Block = request.Level,
                                 Level = request.Level,
                                 Option = (AttributeOption)request.AttributeOption,
                                 Choice = request.Value
                             }
               };
    }

    public static MKAppCharacters.UpgradeRequest ToUpdate(this MKCtrCharacterRequests.UpgradeRemoveRequest request, Guid accountId, Guid characterId)
    {
        return new MKAppCharacters.UpgradeRequest
               {
                   AccountId = accountId,
                   CharacterId = characterId,
                   UpgradeOption = (UpgradeOption)request.UpgradeOption,
                   Upgrade = new MKAppCharacters.Upgrade
                             {
                                 Id = request.UpgradeId,
                                 Block = -1,
                                 Level = -1,
                                 Option = AttributeOption.cunning, // doesn't matter
                                 Choice = request.Value
                             }
               };
    }

    #endregion

    #region Flaws

    public static Flaw ToFlaw(this CreateFlawRequest request)
    {
        return new Flaw
               {
                   Id = request.Id,
                   Name = request.Name,
                   Description = request.Description,
                   IsCustom = request.IsCustom
               };
    }

    public static Flaw ToFlaw(this UpdateFlawRequest request)
    {
        return new Flaw
               {
                   Id = request.Id,
                   Name = request.Name,
                   Description = request.Description,
                   IsCustom = request.IsCustom
               };
    }

    public static FlawResponse ToResponse(this Flaw flaw)
    {
        return new FlawResponse
               {
                   Id = flaw.Id,
                   Name = flaw.Name,
                   Description = flaw.Description,
                   IsCustom = flaw.IsCustom
               };
    }

    public static FlawsResponse ToResponse(this IEnumerable<Flaw> flaws, int page, int pageSize, int total)
    {
        return new FlawsResponse
               {
                   Items = flaws.Select(ToResponse).ToList(),
                   Page = page,
                   PageSize = pageSize,
                   Total = total
               };
    }

    public static GetAllFlawsOptions ToOptions(this GetAllFlawsRequest request)
    {
        return new GetAllFlawsOptions
               {
                   Page = request.Page,
                   PageSize = request.PageSize,
                   SortField = request.SortBy
               };
    }

    #endregion

    #region Humans

    public static MKAppHumans.GetAllHumansOptions ToOptions(this MKCtrHumanRequests.GetAllHumansRequest request)
    {
        return new MKAppHumans.GetAllHumansOptions
               {
                   CharacterId = request.CharacterId,
                   Name = request.Name,
                   Page = request.Page,
                   PageSize = request.PageSize,
                   SortField = request.SortBy,
                   SortOrder = request.SortBy is null ? SortOrder.unordered : request.SortBy.StartsWith('-') ? SortOrder.descending : SortOrder.ascending
               };
    }

    public static HumanResponse ToResponse(this MKAppHumans.Human human)
    {
        return new HumanResponse
               {
                   Id = human.Id,
                   CharacterId = human.CharacterId,
                   Name = human.Name,
                   Description = human.Description,
                   Problems = human.Problems.Select(ToResponse).ToList()
               };
    }

    public static HumansResponse ToGetAllResponse(this IEnumerable<MKAppHumans.Human> humans, int page, int pageSize, int total)
    {
        return new HumansResponse
               {
                   Items = humans.Select(ToResponse).ToList(),
                   Page = page,
                   PageSize = pageSize,
                   Total = total
               };
    }

    public static ProblemResponse ToResponse(this MKAppHumans.Problem problem)
    {
        return new ProblemResponse
               {
                   Id = problem.Id,
                   HumanId = problem.HumanId,
                   Source = problem.Source,
                   Emotion = problem.Emotion,
                   Rank = problem.Rank,
                   Solved = problem.Solved
               };
    }

    public static Application.Models.Humans.Updates.DescriptionUpdate ToUpdate(this MKCtrHumanRequests.HumanDescriptionUpdateRequest request, MKCtrHumanRequests.DescriptionOption description)
    {
        return new Application.Models.Humans.Updates.DescriptionUpdate
               {
                   DescriptionOption = (DescriptionOption)description,
                   HumanId = request.CharacterId,
                   Name = request.Name,
                   Description = request.Description
               };
    }

    public static ProblemUpdate ToUpdate(this MKCtrHumanRequests.HumanProblemUpdateRequest request, MKCtrHumanRequests.ProblemOption problem)
    {
        return new ProblemUpdate
               {
                   ProblemOption = (ProblemOption)problem,
                   HumanId = request.HumanId,
                   ProblemId = request.ProblemId,
                   Source = request.Source,
                   Emotion = request.Emotion,
                   Rank = request.Rank,
                   Solved = request.Solved
               };
    }

    #endregion

    #region Talents

    public static Talent ToTalent(this CreateTalentRequest request)
    {
        return new Talent
               {
                   Id = request.Id,
                   Name = request.Name,
                   Description = request.Description,
                   IsCustom = request.IsCustom
               };
    }

    public static Talent ToTalent(this UpdateTalentRequest request)
    {
        return new Talent
               {
                   Id = request.Id,
                   Name = request.Name,
                   Description = request.Description,
                   IsCustom = request.IsCustom
               };
    }

    public static TalentResponse ToResponse(this Talent talent)
    {
        return new TalentResponse
               {
                   Id = talent.Id,
                   Name = talent.Name,
                   Description = talent.Description,
                   IsCustom = talent.IsCustom
               };
    }

    public static TalentsResponse ToResponse(this IEnumerable<Talent> talents, int page, int pageSize, int total)
    {
        return new TalentsResponse
               {
                   Items = talents.Select(ToResponse).ToList(),
                   Page = page,
                   PageSize = pageSize,
                   Total = total
               };
    }

    public static GetAllTalentsOptions ToOptions(this GetAllTalentsRequest request)
    {
        return new GetAllTalentsOptions
               {
                   Page = request.Page,
                   PageSize = request.PageSize,
                   SortField = request.SortBy
               };
    }

    #endregion

    #region Magical Powers

    public static MagicalPower ToMagicalPower(this CreateMagicalPowerRequest request)
    {
        return new MagicalPower
               {
                   Id = request.Id,
                   Name = request.Name,
                   Description = request.Description,
                   IsCustom = request.IsCustom,
                   BonusFeatures = request.BonusFeatures.Select(ToMagicalPower).ToList()
               };
    }

    public static MagicalPower ToMagicalPower(this UpdateMagicalPowerRequest request)
    {
        return new MagicalPower
               {
                   Id = request.Id,
                   Name = request.Name,
                   Description = request.Description,
                   IsCustom = request.IsCustom,
                   BonusFeatures = request.BonusFeatures.Select(ToMagicalPower).ToList()
               };
    }

    public static MagicalPowerResponse ToResponse(this MagicalPower magicalPower)
    {
        return new MagicalPowerResponse
               {
                   Id = magicalPower.Id,
                   Name = magicalPower.Name,
                   Description = magicalPower.Description,
                   IsCustom = magicalPower.IsCustom,
                   BonusFeatures = magicalPower.BonusFeatures.Select(ToResponse).ToList()
               };
    }

    public static MagicalPowersResponse ToResponse(this IEnumerable<MagicalPower> magicalPowers, int page, int pageSize, int total)
    {
        return new MagicalPowersResponse
               {
                   Items = magicalPowers.Select(ToResponse).ToList(),
                   Page = page,
                   PageSize = pageSize,
                   Total = total
               };
    }

    public static GetAllMagicalPowersOptions ToOptions(this GetAllMagicalPowersRequest request)
    {
        return new GetAllMagicalPowersOptions
               {
                   Page = request.Page,
                   PageSize = request.PageSize,
                   SortField = request.SortBy
               };
    }

    #endregion

    #region GlobalSettings

    public static GlobalSetting ToGlobalSetting(this GlobalSettingCreateRequest request)
    {
        return new GlobalSetting
               {
                   Id = Guid.NewGuid(),
                   Name = request.Name,
                   Value = request.Value
               };
    }

    public static GetAllGlobalSettingsOptions ToOptions(this GetAllGlobalSettingsRequest request)
    {
        string? sortField = request.SortBy?.Trim('+', '-');

        return new GetAllGlobalSettingsOptions
               {
                   Name = request.Name,
                   SortField = sortField,
                   SortOrder = request.SortBy is null ? SortOrder.unordered : request.SortBy.StartsWith('-') ? SortOrder.descending : SortOrder.ascending,
                   Page = request.Page,
                   PageSize = request.PageSize
               };
    }

    public static GlobalSettingResponse ToResponse(this GlobalSetting globalSetting)
    {
        return new GlobalSettingResponse
               {
                   Id = globalSetting.Id,
                   Name = globalSetting.Name,
                   Value = globalSetting.Value
               };
    }

    public static GlobalSettingsResponse ToResponse(this IEnumerable<GlobalSetting> settings, int page, int pageSize, int totalCount)
    {
        return new GlobalSettingsResponse
               {
                   Items = settings.Select(ToResponse),
                   Page = page,
                   PageSize = pageSize,
                   Total = totalCount
               };
    }

    #endregion

    #region GameRules

    public static GameRulesResponse ToResponse(this GameRules rules)
    {
        return new GameRulesResponse
               {
                    MaxLevel = rules.MaxLevel,
                    MinAttributeValue = rules.MinAttributeValue,
                    MaxAttributeValue = rules.MaxAttributeValue,
                    MinInjuries = rules.MinInjuries,
                    MaxInjuries = rules.MaxInjuries,
                    LevelExperiencePoints = rules.LevelExperiencePoints,
                    Attributes = rules.Attributes,
                    Flaws = rules.Flaws.Select(ToResponse),
                    Talents = rules.Talents.Select(ToResponse),
                    MagicalPowers = rules.MagicalPowers.Select(ToResponse),
                    Upgrades =rules.Upgrades.Select(ToResponse),
                    ProblemSource = rules.ProblemSource.Select(ToResponse),
                    Emotion = rules.Emotion.Select(ToResponse),
                    DiceRules = rules.DiceRules,
                    DiceDifficulties = rules.DiceDifficulties.Select(ToResponse),
                    DiceSuccesses = rules.DiceSuccesses.Select(ToResponse),
                    RollComplications = rules.RollComplications,
                    RollBonus = rules.RollBonus,
                    RollSuperBonus = rules.RollSuperBonus,
                    SpendingKittyTreats = rules.SpendingKittyTreats,
                    Healing = rules.Healing,
                    EndOfEpisodeInfo = rules.EndOfEpisodeInfo,
                    EndEpisodeQuestions = rules.EndEpisodeQuestions
               };
    }

    public static UpgradeRuleResponse ToResponse(this MKAppCharacters.UpgradeRule rule)
    {
        return new UpgradeRuleResponse
               {
                   Id = rule.Id,
                   Block = rule.Block,
                   Value = rule.Value
               };
    }

    public static ProblemSourceResponse ToResponse(this ProblemRule.Problem problem)
    {
        return new ProblemSourceResponse
               {
                   RollValue = problem.RollValue,
                   ProblemSource = problem.ProblemSource
               };
    }

    public static EmotionResponse ToResponse(this ProblemRule.Emotion emotion)
    {
        return new EmotionResponse
               {
                   RollValue = emotion.RollValue,
                   EmotionSource = emotion.EmotionSource
               };
    }

    public static DiceDifficultyResponse ToResponse(this DiceRule.DiceDifficulty diceDifficulty)
    {
        return new DiceDifficultyResponse
               {
                   Difficulty = diceDifficulty.Difficulty,
                   DifficultyText = diceDifficulty.DifficultyText,
                   CunningAction = diceDifficulty.CunningAction,
                   CuteAction = diceDifficulty.CuteAction,
                   FierceAction = diceDifficulty.FierceAction
               };
    }

    public static DiceSuccessResponse ToResponse(this DiceRule.DiceSuccess diceSuccess)
    {
        return new DiceSuccessResponse
               {
                   Successes = diceSuccess.Successes,
                   Result = diceSuccess.Result,
                   Enhancements = diceSuccess.Enhancements
               };
    }
    #endregion
}