namespace MagicalKitties.Application;

public static class ApplicationAssumptions
{
    /// <summary>
    /// 0: Username
    /// 1: Activation Code
    /// </summary>
    public const string ActivationLinkFormat = "https://localhost:5001/api/accounts/activate/{0}/{1}";
    
    /// <summary>
    /// 0: Username
    /// 1: Activation Code
    /// </summary>
    public const string ResendActivationLinkFormat = "https://localhost:5001/api/accounts/activate/{0}/{1}/resend";
}