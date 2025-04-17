using MagicalKitties.Application.Models.Auth;

namespace MagicalKitties.Application.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(AccountLogin accountLogin, CancellationToken token = default);
}
