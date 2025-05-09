namespace MagicalKitties.Api;

public static class ApiAssumptions
{
    public static class PolicyNames
    {
        public const string Flaws = "FlawCache";
        public const string MagicalPowers = "MagicalPowerCache";
        public const string RateLimiter = "ThreeRequestsPerFiveSecond";
        public const string Talents = "TalentCache";
        public const string Rules = "RuleCache";
    }

    public static class TagNames
    {
        public const string Flaws = "flaws";
        public const string MagicalPowers = "magicalpowers";
        public const string Talents = "talents";
        public const string Rules = "rules";
    }
}