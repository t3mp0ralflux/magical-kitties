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
        public const string Get = $"{Base}/{{id:guid}}";
        public const string GetAll = Base;
        public const string ResendActivation = $"{Base}/activate/{{username}}/{{activationcode}}/resend";
        public const string Update = Base;
    }

    public static class Auth
    {
        private const string Base = "auth";
        public const string Login = $"{Base}/login";
        public const string PasswordReset = $"{Base}/passwordreset";
        public const string RequestPasswordReset = $"{Base}/passwordreset/{{email}}";
        public const string VerifyPasswordResetCode = $"{Base}/passwordreset/{{email}}/verify";
    }

    public static class Characters
    {
        private const string Base = $"{ApiBase}/characters";
        public const string Create = Base;
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string Get = $"{Base}/{{id:guid}}";
        public const string GetAll = Base;
        public const string UpdateDescription = $"{Base}/description/{{description}}";
        public const string UpdateAttribute = $"{Base}/attributes/{{attribute}}";
        public const string Reset = $"{Base}/{{id:guid}}/reset";
        public const string UpsertUpgrade = $"{Base}/{{characterId:guid}}/upgrade/upsert";
        public const string RemoveUpgrade = $"{Base}/{{characterId:guid}}/upgrade/remove";
    }

    public static class Flaws
    {
        private const string Base = $"{ApiBase}/Flaws";
        public const string Create = Base;
        public const string Delete = $"{Base}/{{id:int}}";
        public const string Get = $"{Base}/{{id:int}}";
        public const string GetAll = Base;
        public const string Update = Base;
    }

    public static class Humans
    {
        private const string Base = $"{ApiBase}/Humans";
        public const string Create = $"{Base}/{{characterId:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string Get = $"{Base}/{{id:guid}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{description}}";
        public const string CreateProblem = $"{Base}/problem/{{humanId:guid}}";
        public const string DeleteProblem = $"{Base}/problem/{{id:guid}}";
        public const string UpdateProblem = $"{Base}/problem/{{problem}}";
    }

    public static class Talents
    {
        private const string Base = $"{ApiBase}/Talents";
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