using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Auth;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.GlobalSettings;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Contracts.Requests.Account;
using MagicalKitties.Contracts.Requests.Auth;
using MagicalKitties.Contracts.Requests.Characters;
using MagicalKitties.Contracts.Requests.Endowments.Flaws;
using MagicalKitties.Contracts.Requests.Endowments.Talents;
using MagicalKitties.Contracts.Requests.GlobalSetting;
using MagicalKitties.Contracts.Responses.Account;
using MagicalKitties.Contracts.Responses.Auth;
using MagicalKitties.Contracts.Responses.Characters;
using MagicalKitties.Contracts.Responses.Flaws;
using MagicalKitties.Contracts.Responses.GlobalSetting;
using MagicalKitties.Contracts.Responses.Talents;
using Attribute = MagicalKitties.Application.Models.Characters.Attribute;
using ctr = MagicalKitties.Contracts.Models;

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
                   AccountRole = (ctr.AccountRole)account.AccountRole,
                   AccountStatus = (ctr.AccountStatus)account.AccountStatus,
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

    public static Character ToCharacter(this CharacterUpdateRequest request, Account account)
    {
        return new Character
               {
                   Id = request.Id,
                   AccountId = account.Id,
                   Name = request.Name,
                   Username = account.Username
               };
    }

    public static CharactersResponse ToGetAllResponse(this IEnumerable<Character> characters, int page, int pageSize, int total)
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

    public static CharacterResponse ToResponse(this Character character)
    {
        return new CharacterResponse
               {
                   Id = character.Id,
                   AccountId = character.AccountId,
                   Name = character.Name,
                   Attributes = character.Attributes.ToResponse(),
                   Flaw = character.Flaw?.ToResponse(),
                   Talent = character.Talent?.ToResponse(),
                   //MagicalPowers = character.MagicalPowers.ToResponse(),
                   CurrentInjuries = character.CurrentInjuries,
                   CurrentOwies = character.CurrentOwies,
                   MaxOwies = character.MaxOwies,
                   CurrentTreats = character.CurrentTreats,
                   StartingTreats = character.StartingTreats,
                   CurrentXp = character.CurrentXp,
                   Description = character.Description,
                   Hometown = character.Hometown,
                   Human = character.Human?.ToResponse(),
                   Level = character.Level
               };
    }

    public static HumanResponse ToResponse(this Human human)
    {
        return new HumanResponse
               {
                   Id = human.Id,
                   Name = human.Name,
                   Description = human.Description,
                   Problems = human.Problems.ToResponse()
               };
    }

    public static ProblemResponse ToResponse(this Problem problem)
    {
        return new ProblemResponse
               {
                   Id = problem.Id,
                   Source = problem.Source,
                   Emotion = problem.Emotion.Value,
                   Rank = problem.Rank,
                   Solved = problem.Solved
               };
    }

    public static List<ProblemResponse> ToResponse(this List<Problem> problems)
    {
        return problems.Select(x => x.ToResponse()).ToList();
    }
    
    public static AttributeResponse ToResponse(this Attribute attribute)
    {
        return new AttributeResponse
               {
                   Id = attribute.Id,
                   Name = attribute.Name,
                   Value = attribute.Value
               };
    }

    public static List<AttributeResponse> ToResponse(this List<Attribute> attributes)
    {
        return attributes.Select(x => x.ToResponse()).ToList();
    }

    public static GetAllCharactersOptions ToOptions(this GetAllCharactersRequest request, Guid accountId)
    {
        return new GetAllCharactersOptions
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

    public static LevelUpdate ToUpdate(this CharacterLevelUpdateRequest request)
    {
        return new LevelUpdate
               {
                   CharacterId = request.CharacterId,
                   Level = request.Level
               };
    }

    public static FlawUpdate ToUpdate(this CharacterFlawUpdateRequest request)
    {
        return new FlawUpdate
               {
                   CharacterId = request.CharacterId,
                   FlawId = request.FlawId
               };
    }

    public static TalentUpdate ToUpdate(this CharacterTalentUpdateRequest request)
    {
        return new TalentUpdate
               {
                   CharacterId = request.CharacterId,
                   TalentId = request.TalentId
               };
    }

    public static CharacterUpdateResponse ToResponse(this LevelUpdate update, string message)
    {
        return new CharacterUpdateResponse
               {
                   CharacterId = update.CharacterId,
                   Message = message
               };
    }

    public static CharacterUpdateResponse ToResponse(this FlawUpdate update, string message)
    {
        return new CharacterUpdateResponse
               {
                   CharacterId = update.CharacterId,
                   Message = message
               };
    }

    public static CharacterUpdateResponse ToResponse(this TalentUpdate update, string message)
    {
        return new CharacterUpdateResponse
               {
                   CharacterId = update.CharacterId,
                   Message = message
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
}