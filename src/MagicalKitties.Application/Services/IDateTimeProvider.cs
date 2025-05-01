namespace MagicalKitties.Application.Services;

public interface IDateTimeProvider
{
    DateTime GetUtcNow();
}