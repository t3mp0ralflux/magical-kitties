namespace MagicalKitties.Application;

public static class ApplicationAssumptions
{
    /// <summary>
    /// 0: Username
    /// 1: Activation Code
    /// </summary>
    public const string ActivationLinkFormat = "https://localhost:5001/api/accounts/activate/{0}/{1}";

    public const string CunningAttributeId = "5337bf7d-0403-4375-aa4d-75280a8f38d9";

    public const string CuteAttributeId = "84f14c16-9569-4254-b0e0-30ded06a1961";
    public const string FierceAttributeId = "72a68a30-c6ba-467b-8e91-872827c97660";

    /// <summary>
    /// 0: Username
    /// 1: Activation Code
    /// </summary>
    public const string ResendActivationLinkFormat = "https://localhost:5001/api/account/activate/{0}/{1}/resend";
}