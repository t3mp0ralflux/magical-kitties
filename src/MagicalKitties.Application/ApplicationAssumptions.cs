namespace MagicalKitties.Application;

public static class ApplicationAssumptions
{
    // TODO: Obviously change these to not be hard-coded to 4200
    
    /// <summary>
    /// 0: Username
    /// 1: Activation Code
    /// </summary>
    public const string ActivationLinkFormat = "https://localhost:4200/register/activation/{0}";

    /// <summary>
    /// 0: Username
    /// 1: Activation Code
    /// </summary>
    public const string ResendActivationLinkFormat = "https://localhost:4200/registeraccounts/activation/{0}/resend";

    /// <summary>
    /// 0: Username
    /// 1: Activation Code
    /// </summary>
    public const string PasswordResetLinkFormat = "https://localhost:4200/login/reset-password/{0}";
}