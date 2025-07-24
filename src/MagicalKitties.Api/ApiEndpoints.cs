namespace MagicalKitties.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Accounts
    {
        public const string Activate = $"{Base}/activate/{{username}}/{{activationcode}}";
        private const string Base = $"{ApiBase}/accounts";
        public const string Create = Base;
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string Get = $"{Base}/{{emailOrId}}";
        public const string GetAll = Base;
        public const string ResendActivation = $"{Base}/activate/{{username}}/{{activationcode}}/resend";
        public const string Update = Base;
    }

    public static class Auth
    {
        private const string Base = "auth";
        public const string Login = $"{Base}/login";
        public const string Logout = $"{Base}/logout/{{accountId:guid}}";
        public const string LoginByToken = $"{Base}/login/token";
        public const string RefreshToken = $"{Base}/token/refresh";
        public const string PasswordReset = $"{Base}/passwordreset";
        public const string RequestPasswordReset = $"{Base}/passwordreset/request";
        public const string VerifyPasswordResetCode = $"{Base}/passwordreset/verify";
    }

    public static class Characters
    {
        private const string Base = $"{ApiBase}/characters";
        public const string Create = Base;
        public const string Copy = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string Get = $"{Base}/{{id:guid}}";
        public const string GetAll = Base;
        public const string RemoveUpgrade = $"{Base}/{{characterId:guid}}/upgrade/remove";
        public const string Reset = $"{Base}/{{id:guid}}/reset";
        public const string UpdateAttribute = $"{Base}/attributes/{{attribute}}";
        public const string UpdateDescription = $"{Base}/description/{{description}}";
        public const string UpsertUpgrade = $"{Base}/{{characterId:guid}}/upgrade/upsert";
    }

    public static class Flaws
    {
        private const string Base = $"{ApiBase}/flaws";
        public const string Create = Base;
        public const string Delete = $"{Base}/{{id:int}}";
        public const string Get = $"{Base}/{{id:int}}";
        public const string GetAll = Base;
        public const string Update = Base;
    }

    public static class Humans
    {
        private const string Base = $"{ApiBase}/humans";
        public const string Create = $"{Base}/{{characterId:guid}}";
        public const string CreateProblem = $"{Base}/{{humanId:guid}}/problem";
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string DeleteProblem = $"{Base}/{{humanId:guid}}/problem/{{problemId:guid}}";
        public const string Get = $"{Base}/{{id:guid}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{description}}";
        public const string UpdateProblem = $"{Base}/problem/{{problem}}";
    }

    public static class Rules
    {
        private const string Base = $"{ApiBase}/rules";
        public const string GetAll = Base;
    }

    public static class Talents
    {
        private const string Base = $"{ApiBase}/talents";
        public const string Create = Base;
        public const string Delete = $"{Base}/{{id:int}}";
        public const string Get = $"{Base}/{{id:int}}";
        public const string GetAll = Base;
        public const string Update = Base;
    }

    public static class MagicalPowers
    {
        private const string Base = $"{ApiBase}/MagicalPowers";
        public const string Create = Base;
        public const string Delete = $"{Base}/{{id:int}}";
        public const string Get = $"{Base}/{{id:int}}";
        public const string GetAll = Base;
        public const string Update = Base;
    }

    public static class GlobalSettings
    {
        private const string Base = $"{ApiBase}/globalsettings";
        public const string Create = Base;
        public const string Get = $"{Base}/{{name}}";
        public const string GetAll = Base;
    }
}